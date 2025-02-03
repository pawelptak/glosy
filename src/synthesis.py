import argparse
from model_loader import load_model

def synthesize_text(text, speaker_wav, language, file_path, model_name):
    tts = load_model(model_name)
    
    tts.tts_to_file(
        text=text,
        speaker_wav=speaker_wav,
        language=language,
        file_path=file_path
    )
    print(f"Synthesis completed. Output saved to {file_path}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Synthesis Script")
    parser.add_argument("model_name", type=str, help="The name of the TTS model to use")
    parser.add_argument("text", type=str, help="Text to synthesize")
    parser.add_argument("speaker_wav", type=str, help="Path to the speaker WAV file")
    parser.add_argument("language", type=str, help="Language of the speech")
    parser.add_argument("file_path", type=str, help="Path to save the synthesized file")

    args = parser.parse_args()

    synthesize_text(args.text, args.speaker_wav, args.language, args.file_path, args.model_name)
