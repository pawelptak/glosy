## Scripts usage
```
python conversion.py voice_conversion_models/multilingual/multi-dataset/openvoice_v2 source.wav target.wav output.wav

python synthesis.py tts_models/multilingual/multi-dataset/xtts_v2 "Witaj świecie." target.wav output.wav --language pl
```

## Models supported
### Synthesis
- `tts_models/multilingual/multi-dataset/xtts_v2`
- `tts_models/multilingual/multi-dataset/xtts_v1.1`
- `tts_models/multilingual/multi-dataset/bark` (do not provide the language parameter)
- `tts_models/pl/mai_female/vits` (do not provide the language parameter)

### Conversion
- `voice_conversion_models/multilingual/multi-dataset/openvoice_v2`
- `voice_conversion_models/multilingual/multi-dataset/openvoice_v1`
- `voice_conversion_models/multilingual/vctk/freevc24`