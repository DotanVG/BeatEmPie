"""
BeatEmPie — Procedural Chiptune Soundtrack Generator
Generates 6 WAV tracks using pure Python + numpy (no external audio tools needed).
Style: 8-bit/16-bit chiptune — square waves, triangle waves, noise percussion.

Usage:
    python generate_music.py

To regenerate a single track:
    python generate_music.py --track bgm_mainmenu
"""

import numpy as np
import wave
import struct
import argparse
import os

SAMPLE_RATE = 44100
CHANNELS    = 2  # stereo


# ── Waveform Generators ────────────────────────────────────────────────────────

def square(freq, duration, sr=SAMPLE_RATE, duty=0.5, vol=0.3):
    t = np.linspace(0, duration, int(sr * duration), endpoint=False)
    wave_ = np.where((t * freq % 1) < duty, 1.0, -1.0) * vol
    return wave_.astype(np.float32)

def triangle(freq, duration, sr=SAMPLE_RATE, vol=0.3):
    t = np.linspace(0, duration, int(sr * duration), endpoint=False)
    phase = (t * freq % 1)
    wave_ = (2 * np.abs(2 * phase - 1) - 1) * vol
    return wave_.astype(np.float32)

def sine(freq, duration, sr=SAMPLE_RATE, vol=0.3):
    t = np.linspace(0, duration, int(sr * duration), endpoint=False)
    return (np.sin(2 * np.pi * freq * t) * vol).astype(np.float32)

def noise_burst(duration, sr=SAMPLE_RATE, vol=0.4, decay=8.0):
    n = int(sr * duration)
    t = np.linspace(0, duration, n, endpoint=False)
    env = np.exp(-decay * t)
    return (np.random.uniform(-1, 1, n) * env * vol).astype(np.float32)

def silence(duration, sr=SAMPLE_RATE):
    return np.zeros(int(sr * duration), dtype=np.float32)

def apply_envelope(sig, attack=0.005, release=0.01, sr=SAMPLE_RATE):
    a = min(int(attack * sr), len(sig))
    r = min(int(release * sr), len(sig))
    sig[:a] *= np.linspace(0, 1, a)
    sig[-r:] *= np.linspace(1, 0, r)
    return sig


# ── Note helpers ───────────────────────────────────────────────────────────────

NOTE_FREQS = {
    'C2':65.41,'D2':73.42,'E2':82.41,'F2':87.31,'G2':98.00,'A2':110.00,'B2':123.47,
    'C3':130.81,'D3':146.83,'E3':164.81,'F3':174.61,'G3':196.00,'A3':220.00,'Bb3':233.08,'B3':246.94,
    'C4':261.63,'D4':293.66,'Eb4':311.13,'E4':329.63,'F4':349.23,'G4':392.00,'A4':440.00,'Bb4':466.16,'B4':493.88,
    'C5':523.25,'D5':587.33,'Eb5':622.25,'E5':659.25,'F5':698.46,'G5':783.99,'A5':880.00,'Bb5':932.33,'B5':987.77,
    'C6':1046.50,'D6':1174.66,'E6':1318.51,'F6':1396.91,'G6':1567.98,'A6':1760.00,
    'REST':0.0,
}

def note(name, dur, gen=square, vol=0.25, **kwargs):
    f = NOTE_FREQS.get(name, 0.0)
    if f == 0.0:
        return silence(dur)
    sig = gen(f, dur, vol=vol, **kwargs)
    return apply_envelope(sig)

def seq(notes_durs, gen=square, vol=0.25, **kwargs):
    """Build a sequence from [(note_name, duration), ...]"""
    parts = []
    for n, d in notes_durs:
        parts.append(note(n, d, gen=gen, vol=vol, **kwargs))
    return np.concatenate(parts)


# ── Drums ──────────────────────────────────────────────────────────────────────

def kick(dur=0.12):
    n = int(SAMPLE_RATE * dur)
    t = np.linspace(0, dur, n)
    freq = 120 * np.exp(-30 * t)
    sig  = np.sin(2 * np.pi * freq * t) * np.exp(-20 * t) * 0.7
    return sig.astype(np.float32)

def snare(dur=0.10):
    n   = int(SAMPLE_RATE * dur)
    t   = np.linspace(0, dur, n)
    tone  = np.sin(2 * np.pi * 200 * t) * np.exp(-30 * t) * 0.3
    noise_ = np.random.uniform(-1, 1, n) * np.exp(-25 * t) * 0.5
    return (tone + noise_).astype(np.float32)

def hihat(dur=0.06, vol=0.15):
    n = int(SAMPLE_RATE * dur)
    t = np.linspace(0, dur, n)
    sig = np.random.uniform(-1, 1, n) * np.exp(-60 * t) * vol
    return sig.astype(np.float32)

def crash(dur=0.4, vol=0.35):
    n = int(SAMPLE_RATE * dur)
    t = np.linspace(0, dur, n)
    sig = np.random.uniform(-1, 1, n) * np.exp(-8 * t) * vol
    return sig.astype(np.float32)

def timpani_hit(dur=0.5, vol=0.5):
    n = int(SAMPLE_RATE * dur)
    t = np.linspace(0, dur, n)
    freq = 60 * np.exp(-2 * t)
    sig  = np.sin(2 * np.pi * freq * t) * np.exp(-5 * t) * vol
    return sig.astype(np.float32)

def pad_or_trim(a, length):
    if len(a) >= length:
        return a[:length]
    return np.concatenate([a, np.zeros(length - len(a), dtype=np.float32)])

def mix(*arrays):
    length = max(len(a) for a in arrays)
    result = np.zeros(length, dtype=np.float32)
    for a in arrays:
        result[:len(a)] += a
    return np.clip(result, -1.0, 1.0)

def repeat(sig, times):
    return np.tile(sig, times)

def to_stereo(mono, pan=0.0):
    """pan: -1 (full left) to +1 (full right), 0 = center"""
    l = mono * (1.0 - max(0, pan))
    r = mono * (1.0 + min(0, pan))
    stereo = np.empty(len(mono) * 2, dtype=np.float32)
    stereo[0::2] = l
    stereo[1::2] = r
    return stereo

def save_wav(filename, mono_signal, sr=SAMPLE_RATE):
    stereo = to_stereo(mono_signal)
    stereo = np.clip(stereo, -1.0, 1.0)
    data   = (stereo * 32767).astype(np.int16).tobytes()
    with wave.open(filename, 'w') as wf:
        wf.setnchannels(2)
        wf.setsampwidth(2)
        wf.setframerate(sr)
        wf.writeframes(data)
    kb = os.path.getsize(filename) // 1024
    print(f"  Saved {filename} ({kb} KB)")


# ══════════════════════════════════════════════════════════════════════════════
# TRACK 1 — bgm_mainmenu.wav  "Pie Shop"
# BPM 90, C major, cheerful accordion-feel
# ══════════════════════════════════════════════════════════════════════════════

def gen_mainmenu():
    bpm  = 90
    beat = 60 / bpm
    h    = beat / 2   # half beat
    q    = beat       # quarter
    e    = beat / 4   # eighth

    # Melody (square, accordion-y with duty=0.3)
    mel_bar1 = seq([
        ('C5',q),('E5',q),('G5',q),('C6',q),
    ], gen=square, vol=0.28, duty=0.3)
    mel_bar2 = seq([
        ('B4',q),('D5',q),('G5',h),
    ], gen=square, vol=0.28, duty=0.3)
    mel_bar3 = seq([
        ('A4',q),('C5',q),('E5',q),('G5',q),
    ], gen=square, vol=0.28, duty=0.3)
    mel_bar4 = seq([
        ('G4',h),('C5',h),
    ], gen=square, vol=0.28, duty=0.3)

    phrase = np.concatenate([mel_bar1, mel_bar2, mel_bar3, mel_bar4])

    # Counter melody (triangle)
    cnt_bar1 = seq([('E4',q),('G4',q),('C5',h)], gen=triangle, vol=0.18)
    cnt_bar2 = seq([('D4',q),('F4',q),('B4',h)], gen=triangle, vol=0.18)
    cnt_bar3 = seq([('C4',q),('E4',q),('A4',h)], gen=triangle, vol=0.18)
    cnt_bar4 = seq([('G3',q),('B3',q),('C4',h)], gen=triangle, vol=0.18)
    counter  = np.concatenate([cnt_bar1, cnt_bar2, cnt_bar3, cnt_bar4])

    # Bass (triangle, bouncy)
    bass_bar = seq([('C3',h),('G3',h)], gen=triangle, vol=0.25)
    bass_bar2 = seq([('G2',h),('D3',h)], gen=triangle, vol=0.25)
    bass_bar3 = seq([('A2',h),('E3',h)], gen=triangle, vol=0.25)
    bass_bar4 = seq([('C3',q),('E3',q),('G3',h)], gen=triangle, vol=0.25)
    bass      = np.concatenate([bass_bar, bass_bar2, bass_bar3, bass_bar4])

    # Percussion (4 bars = 16 beats)
    def drum_bar():
        k = pad_or_trim(kick(), int(SAMPLE_RATE * q))
        s = pad_or_trim(snare(), int(SAMPLE_RATE * q))
        hh = pad_or_trim(hihat(), int(SAMPLE_RATE * e))
        hh2 = np.concatenate([hh, hh, hh, hh, hh, hh, hh, hh])
        bar = mix(
            np.concatenate([k, silence(q), s, silence(q)]),
            hh2
        )
        return bar

    drums = repeat(drum_bar(), 4)
    phrase_len = len(phrase)

    track = mix(
        pad_or_trim(phrase, phrase_len),
        pad_or_trim(counter, phrase_len),
        pad_or_trim(bass, phrase_len),
        pad_or_trim(drums, phrase_len),
    )
    # 4 reps for a full 32-bar feel
    return repeat(track, 4)


# ══════════════════════════════════════════════════════════════════════════════
# TRACK 2 — bgm_gameplay_calm.wav  "Streets of Dough"
# BPM 100, A minor, funky + jazzy
# ══════════════════════════════════════════════════════════════════════════════

def gen_gameplay_calm():
    bpm  = 100
    beat = 60 / bpm
    h    = beat / 2
    q    = beat
    e    = beat / 4

    # Lead (square, mellow)
    mel = seq([
        ('A4',q),('C5',q),('E5',h),
        ('G4',q),('Bb4',q),('D5',h),
        ('A4',e),('B4',e),('C5',q),('E5',h),
        ('D5',h),('A4',h),
    ], gen=square, vol=0.22, duty=0.25)

    # Vibraphone-feel counter (sine)
    vib = seq([
        ('E4',h),('A4',h),
        ('D4',h),('G4',h),
        ('C4',h),('E4',h),
        ('A3',q),('C4',q),('E4',h),
    ], gen=sine, vol=0.20)

    # Walking bass (triangle)
    bass = seq([
        ('A2',q),('C3',q),('E3',q),('G3',q),
        ('G2',q),('Bb2',q),('D3',q),('F3',q),
        ('F2',q),('A2',q),('C3',q),('E3',q),
        ('E2',q),('G2',q),('A2',q),('E2',q),
    ], gen=triangle, vol=0.28)

    # Drums — laid back shuffle feel
    def calm_bar():
        k  = pad_or_trim(kick(0.10), int(SAMPLE_RATE * q))
        s  = pad_or_trim(snare(0.08), int(SAMPLE_RATE * q))
        hh = pad_or_trim(hihat(0.04, 0.10), int(SAMPLE_RATE * e))
        groove = np.concatenate([
            mix(k, hh), mix(silence(e), hh),
            mix(s, hh), mix(silence(e), hh),
            mix(k, hh), mix(silence(e), hh),
            mix(s, hh), mix(silence(e), hh),
        ])
        return groove

    drums = repeat(calm_bar(), 4)
    L = max(len(mel), len(vib), len(bass), len(drums))

    track = mix(
        pad_or_trim(mel, L),
        pad_or_trim(vib, L),
        pad_or_trim(bass, L),
        pad_or_trim(drums, L),
    )
    return repeat(track, 4)


# ══════════════════════════════════════════════════════════════════════════════
# TRACK 3 — bgm_gameplay_intense.wav  "Pie Storm"
# BPM 140, D minor, driving action
# ══════════════════════════════════════════════════════════════════════════════

def gen_gameplay_intense():
    bpm  = 140
    beat = 60 / bpm
    h    = beat / 2
    q    = beat
    e    = beat / 4

    # Intense lead (square, distorted duty)
    mel = seq([
        ('D5',e),('F5',e),('A5',e),('D5',e), ('C5',e),('Eb5',e),('G5',e),('C5',e),
        ('Bb4',e),('D5',e),('F5',e),('Bb4',e), ('A4',e),('C5',e),('E5',e),('A4',e),
        ('D5',e),('F5',e),('A5',e),('F5',e), ('G5',e),('Bb5',e),('D6',e),('Bb5',e),
        ('A5',h),('D5',h),
    ], gen=square, vol=0.30, duty=0.15)

    # Rhythm guitar stabs (square, short)
    def stab(n):
        f = NOTE_FREQS.get(n, 0)
        sig = square(f, e * 0.6, vol=0.20, duty=0.5) if f > 0 else silence(e)
        return pad_or_trim(sig, int(SAMPLE_RATE * e))

    rhythm_bar = np.concatenate([
        stab('D4'), stab('REST'), stab('F4'), stab('REST'),
        stab('D4'), stab('REST'), stab('G4'), stab('A4'),
        stab('D4'), stab('REST'), stab('F4'), stab('REST'),
        stab('C4'), stab('Eb4'), stab('G4'), stab('REST'),
    ])
    rhythm = repeat(rhythm_bar, 2)

    # Bass (triangle, aggressive)
    bass = seq([
        ('D2',e),('D2',e),('A2',e),('D2',e), ('C2',e),('C2',e),('G2',e),('C2',e),
        ('Bb1',e) if 'Bb1' in NOTE_FREQS else ('D2',e),
        ('D2',e),('F2',e),('D2',e), ('A1',e) if 'A1' in NOTE_FREQS else ('A2',e),
        ('E2',e),('G2',e),('A2',e),
    ], gen=triangle, vol=0.35)

    # Hard driving drums
    def intense_bar():
        k  = pad_or_trim(kick(0.08), int(SAMPLE_RATE * e))
        s  = pad_or_trim(snare(0.07), int(SAMPLE_RATE * e))
        hh = pad_or_trim(hihat(0.03, 0.12), int(SAMPLE_RATE * e))
        bar = np.concatenate([
            mix(k, hh), mix(hh), mix(s, hh), mix(hh),
            mix(k, hh), mix(k, hh), mix(s, hh), mix(hh),
            mix(k, hh), mix(hh), mix(s, hh), mix(hh),
            mix(k, hh), mix(hh), mix(s, hh), mix(hh),
        ])
        return bar

    drums = repeat(intense_bar(), 4)
    L = max(len(mel), len(rhythm), len(bass), len(drums))

    track = mix(
        pad_or_trim(mel, L),
        pad_or_trim(rhythm, L),
        pad_or_trim(bass, L),
        pad_or_trim(drums, L),
    )
    return repeat(track, 4)


# ══════════════════════════════════════════════════════════════════════════════
# TRACK 4 — bgm_boss.wav  "The Whale Rises"
# BPM 130, E minor, epic + threatening
# ══════════════════════════════════════════════════════════════════════════════

def gen_boss():
    bpm  = 130
    beat = 60 / bpm
    h    = beat / 2
    q    = beat
    e    = beat / 4
    dq   = beat * 1.5  # dotted quarter

    # Ominous brass stabs (square, wide duty)
    brass = seq([
        ('E4', e), ('REST', e), ('E4', e), ('REST', e),
        ('B4', q), ('REST', q),
        ('G4', e), ('REST', e), ('G4', e), ('REST', e),
        ('D5', q), ('REST', q),
        ('E4', e), ('REST', e), ('F4', e), ('REST', e),
        ('G4', q), ('A4', q),
        ('B4', h), ('REST', h),
    ], gen=square, vol=0.28, duty=0.6)

    # Low menacing lead (triangle)
    mel = seq([
        ('E3', q), ('G3', q), ('B3', q), ('E4', q),
        ('D3', q), ('F3', q), ('A3', q), ('D4', q),
        ('C3', q), ('E3', q), ('G3', q), ('C4', q),
        ('B2', q), ('D3', q), ('G3', h),
    ], gen=triangle, vol=0.25)

    # Thunderous bass (sine for weight)
    bass = seq([
        ('E2', h), ('B2', h),
        ('D2', h), ('A2', h),
        ('C2', h), ('G2', h),
        ('B1',q) if 'B1' in NOTE_FREQS else ('B2',q), ('B2',q), ('E2',h),
    ], gen=sine, vol=0.40)

    # Drums — heavy, stomp feel
    def boss_bar():
        k   = pad_or_trim(kick(0.15), int(SAMPLE_RATE * q))
        s   = pad_or_trim(snare(0.12), int(SAMPLE_RATE * q))
        tim = pad_or_trim(timpani_hit(), int(SAMPLE_RATE * q))
        hh  = pad_or_trim(hihat(0.05, 0.08), int(SAMPLE_RATE * e))
        bar = mix(
            np.concatenate([k, silence(q), s, k, k, silence(q), s, silence(q)]),
            pad_or_trim(repeat(hh, 16), int(SAMPLE_RATE * q * 4)),
        )
        # Timpani on beat 1 of every 2 bars — handled via repeat
        return bar

    def boss_bar2():
        k   = pad_or_trim(kick(0.15), int(SAMPLE_RATE * q))
        s   = pad_or_trim(snare(0.12), int(SAMPLE_RATE * q))
        tim = pad_or_trim(timpani_hit(), int(SAMPLE_RATE * h))
        hh  = pad_or_trim(hihat(0.05, 0.08), int(SAMPLE_RATE * e))
        bar = mix(
            np.concatenate([k, silence(q), s, k, k, silence(q), s, silence(q)]),
            pad_or_trim(repeat(hh, 16), int(SAMPLE_RATE * q * 4)),
            np.concatenate([tim, silence(h)]),
        )
        return bar

    drums = np.concatenate([boss_bar(), boss_bar2(), boss_bar(), boss_bar2()])
    L = max(len(brass), len(mel), len(bass), len(drums))

    track = mix(
        pad_or_trim(brass, L),
        pad_or_trim(mel, L),
        pad_or_trim(bass, L),
        pad_or_trim(drums, L),
    )
    return repeat(track, 4)


# ══════════════════════════════════════════════════════════════════════════════
# TRACK 5 — bgm_victory.wav  "Pie Smash!"
# BPM 120, C major, triumphant fanfare — 8 bars
# ══════════════════════════════════════════════════════════════════════════════

def gen_victory():
    bpm  = 120
    beat = 60 / bpm
    h    = beat / 2
    q    = beat
    e    = beat / 4
    dq   = beat * 1.5

    # Trumpet fanfare (square)
    fanfare = seq([
        ('C5',e),('E5',e),('G5',q), ('C6',h),
        ('G5',e),('E5',e),('C5',q), ('G4',h),
        ('C5',e),('E5',e),('G5',e),('A5',e), ('G5',dq),('E5',e),
        ('C5',q),('E5',q),('G5',q), ('C6',h+q),
    ], gen=square, vol=0.32, duty=0.4)

    # Marimba (triangle, sparkly)
    marimba = seq([
        ('E4',e),('G4',e),('C5',e),('E5',e), ('G5',h),
        ('D4',e),('F4',e),('A4',e),('D5',e), ('F5',h),
        ('E4',e),('G4',e),('C5',e),('E5',e), ('G5',e),('A5',e),
        ('G5',q),('E5',q),('C5',q), ('G4',h+q),
    ], gen=triangle, vol=0.22)

    # Bass (triangle)
    bass = seq([
        ('C3',q),('G3',q),('C4',h),
        ('G2',q),('D3',q),('G3',h),
        ('F2',q),('C3',q),('F3',h),
        ('C3',h),('G3',q),('C4',q+h),
    ], gen=triangle, vol=0.28)

    # Drums — crash + punch
    def vic_bar():
        k = pad_or_trim(kick(0.15), int(SAMPLE_RATE * q))
        s = pad_or_trim(snare(0.10), int(SAMPLE_RATE * q))
        c = pad_or_trim(crash(0.5), int(SAMPLE_RATE * q))
        bar = np.concatenate([
            mix(k, c), mix(silence(q)), mix(s), mix(k),
        ])
        return bar

    drums = np.concatenate([vic_bar(), vic_bar(),
                             vic_bar(), vic_bar()])

    L = max(len(fanfare), len(marimba), len(bass), len(drums))

    return mix(
        pad_or_trim(fanfare, L),
        pad_or_trim(marimba, L),
        pad_or_trim(bass, L),
        pad_or_trim(drums, L),
    )


# ══════════════════════════════════════════════════════════════════════════════
# TRACK 6 — bgm_gameover.wav  "The Pies Are Gone"
# BPM 60, C minor, sad + drooping — 6 bars
# ══════════════════════════════════════════════════════════════════════════════

def gen_gameover():
    bpm  = 60
    beat = 60 / bpm
    h    = beat / 2
    q    = beat
    e    = beat / 4

    # Drooping melody (sine, soft)
    mel = seq([
        ('G4', q), ('Eb4', q), ('C4', h),
        ('G3', q), ('Bb3', q), ('Eb4', h),
        ('F4', q), ('Eb4', q), ('D4', q), ('C4', q),
        ('G3', h), ('C3', h),
        ('Bb3', q), ('G3', q), ('Eb3', h),
        ('C3', beat * 3),
    ], gen=sine, vol=0.28)

    # Tuba low tones (triangle, sad)
    tuba = seq([
        ('C2', h), ('G2', h),
        ('Eb2', h), ('Bb2', h),
        ('F2', h), ('C2', h),
        ('G1', q) if 'G1' in NOTE_FREQS else ('G2', q), ('G2', q), ('C2', h),
        ('Bb1', h) if 'Bb1' in NOTE_FREQS else ('Bb2', h), ('Eb2', h),
        ('C2', beat * 3),
    ], gen=triangle, vol=0.30)

    # Slow, sad single drum hit per bar
    def sad_bar():
        k = pad_or_trim(kick(0.25), int(SAMPLE_RATE * beat * 4))
        return k * 0.4

    drums = repeat(sad_bar(), 3)
    L = max(len(mel), len(tuba), len(drums))

    return mix(
        pad_or_trim(mel, L),
        pad_or_trim(tuba, L),
        pad_or_trim(drums, L),
    )


# ── Main ───────────────────────────────────────────────────────────────────────

TRACKS = {
    'bgm_mainmenu':        ('Pie Shop',          gen_mainmenu),
    'bgm_gameplay_calm':   ('Streets of Dough',  gen_gameplay_calm),
    'bgm_gameplay_intense':('Pie Storm',         gen_gameplay_intense),
    'bgm_boss':            ('The Whale Rises',   gen_boss),
    'bgm_victory':         ('Pie Smash!',        gen_victory),
    'bgm_gameover':        ('The Pies Are Gone', gen_gameover),
}

def generate(track_key):
    title, fn = TRACKS[track_key]
    out = os.path.join(os.path.dirname(__file__), f'{track_key}.wav')
    print(f"Generating [{track_key}] — {title}...")
    sig = fn()
    save_wav(out, sig)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--track', choices=list(TRACKS.keys()), default=None,
                        help='Generate a single track (default: all)')
    args = parser.parse_args()

    os.makedirs(os.path.dirname(os.path.abspath(__file__)) or '.', exist_ok=True)

    if args.track:
        generate(args.track)
    else:
        for key in TRACKS:
            generate(key)

    print("\nDone! All tracks saved to Assets/Audio/Music/")
