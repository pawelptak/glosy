<p align="center">
  <img height="100" src="src/Glosy/wwwroot/images/logo.svg">
</p>

# Glosy
Voice conversion and synthesis web app

## Requirements
- Python 3.12 (3.11 should work as well)
- FFmpeg [[FFmpeg Download]](https://ffmpeg.org/download.html)
- .NET SDK version 8.0.XXX [[Download]](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (This may be optional but version 9.0.XXX failed for me on amd64 architecture. If you want to use the newest version, delete the `src/Glosy/global.json` file)
- PyTorch version that matches you CUDA version (more info [here](https://pytorch.org/get-started/locally/)). In my case:
    ```
    pip3 install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu126
    ```
- (Optional) Create and activate Python venv:
  ```
  python -m venv path/to/venv/folder
  path/to/venv/folder/Scripts/activate
  ```
- Install all requirements. Inside `src/Glosy`:
  ```
  pip install -r requirements.txt
  ```
- Edit the `src/Glosy/appsettings.json` and point to the locations of your ffmpeg and python (.exe file paths on Windows/`ffmpeg` and `python` values on Linux)
- Edit the environment variables of your system and add a variable named `TTS_HOME` that will contain the absolute path of the `..\glosy\src\TTS\models\` folder (On Windows put the path without any quotes).
- Download the `checkpoint.pth` file of the desired model and put it inside the `TTS/models/tts/[model_dir]` directory. URLs to the model files can be found here: https://github.com/idiap/coqui-ai-TTS/blob/dev/TTS/.models.json. For example for the openvoice_v1, the file can be found under https://huggingface.co/myshell-ai/OpenVoice/resolve/main/checkpoints/converter/checkpoint.pth.

## Credits
This app is mainly based on https://github.com/idiap/coqui-ai-TTS which is a fork of https://github.com/coqui-ai/TTS.
