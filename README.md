# Glosy
Voice conversion and synthesis web app

## Requirements
- Python 3.12
- PyTorch version that matches you CUDA version (more info [here](https://pytorch.org/get-started/locally/)). In my case:
    ```
    pip3 install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu126
    ```
- (Optional) Create and activate Python venv:
  ```
  python -m venv path/to/venv/folder
  path/to/venv/folder/Scripts/activate
  ```
- Install all requirements:
  ```
  pip install -r requirements.txt
  ```
- Edit the environment variables of your system and add a variable named `TTS_HOME` that will contain the absolute path of the `..\glosy\src\TTS\models\` folder (On Windows put the path without any quotes).
- Download the `checkpoint.pth` file of the desired model and put it inside the `TTS/models/tts/[model_dir]` directory. URLs to the model files can be found here: https://github.com/idiap/coqui-ai-TTS/blob/dev/TTS/.models.json. For example for the openvoice_v1, the file can be found under https://huggingface.co/myshell-ai/OpenVoice/resolve/main/checkpoints/converter/checkpoint.pth.

## Usage
```
python conversion.py voice_conversion_models/multilingual/multi-dataset/openvoice_v2 test_audio_files/ja.wav test_audio_files/nr.wav test_audio_files/out.wav

python synthesis.py tts_models/multilingual/multi-dataset/xtts_v2 "Witaj świecie." test_audio_files/ja.wav pl test_audio_files/out.wav
```

## Credits
This app is mainly based on https://github.com/idiap/coqui-ai-TTS which is a fork of https://github.com/coqui-ai/TTS.
