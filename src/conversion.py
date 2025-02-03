import argparse
from model_loader import load_model

def convert_voice(source_wav, target_wav, file_path, model_name):
    tts = load_model(model_name)
    
    tts.voice_conversion_to_file(
        source_wav=source_wav,
        target_wav=target_wav,
        file_path=file_path
    )
    print(f"Conversion completed. Output saved to {file_path}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Voice Conversion Script")
    parser.add_argument("model_name", type=str, help="The name of the TTS model to use")
    parser.add_argument("source_wav", type=str, help="Path to the source WAV file")
    parser.add_argument("target_wav", type=str, help="Path to the target WAV file")
    parser.add_argument("file_path", type=str, help="Path to save the converted file")

    args = parser.parse_args()

    convert_voice(args.source_wav, args.target_wav, args.file_path, args.model_name)
