import torch
from TTS.api import TTS

def load_model(model_name: str):
    device = "cuda" if torch.cuda.is_available() else "cpu"
    print(f'Using device: {device}')
    tts = TTS(model_name).to(device)

    return tts
