## Scripts usage
```
python conversion.py voice_conversion_models/multilingual/multi-dataset/openvoice_v2 test_audio_files/ja.wav test_audio_files/nr.wav test_audio_files/out.wav

python synthesis.py tts_models/multilingual/multi-dataset/xtts_v2 "Witaj świecie." test_audio_files/ja.wav test_audio_files/out.wav --language pl
```

## Models supported
### Synthesis
- `tts_models/multilingual/multi-dataset/xtts_v2`
- `tts_models/multilingual/multi-dataset/bark` (do not provide the language parameter)

### Conversion
TODO