import torch
from TTS.api import TTS

# Get device
device = "cuda" if torch.cuda.is_available() else "cpu"

# tts = TTS("voice_conversion_models/multilingual/vctk/freevc24").to(device)
tts = TTS("voice_conversion_models/multilingual/multi-dataset/openvoice_v2").to(device)

tts.voice_conversion_to_file(
  source_wav="ja.wav",
  target_wav="target.wav",
  file_path="output_rev.wav"
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