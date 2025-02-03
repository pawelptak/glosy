import torch
from TTS.api import TTS
import os

class Model:
    def __init__(self, model_folder):
        model_path = os.path.join(model_folder, 'checkpoint.pth')
        config_path = os.path.join(model_folder, 'config.json')
        self.model_path = model_path
        self.config_path = config_path

# Get device
device = "cuda" if torch.cuda.is_available() else "cpu"
print(f'Device: {device}')

#model = Model(model_folder='TTS/models/openvoice_v2')

model_name = "voice_conversion_models/multilingual/vctk/freevc24"
model_name = "voice_conversion_models/multilingual/multi-dataset/openvoice_v2" # chyba best. gorszy wynik dla dluzszego targetu
model_name = "voice_conversion_models/multilingual/multi-dataset/openvoice_v1"

#tts = TTS("voice_conversion_models/multilingual/multi-dataset/knnvc").to(device)

tts = TTS(model_name).to(device)
tts.voice_conversion_to_file(
  source_wav="test_audio_files/ja.wav",
  target_wav="test_audio_files/nr.wav",
  file_path="test_audio_files/testest.wav"
)


# tts = TTS("tts_models/de/thorsten/tacotron2-DDC")

# tts.tts_with_vc_to_file(
#   "Wie sage ich auf Italienisch, dass ich dich liebe?",
#   speaker_wav=["ja.wav"],
#   file_path="output.wav"
# )


# tts = TTS("tts_models/multilingual/multi-dataset/xtts_v2").to(device)

# tts.tts_to_file(
#   text="Hello world!",
#   speaker_wav="ja.wav",
#   language="en",
#   file_path="output.wav"
# )