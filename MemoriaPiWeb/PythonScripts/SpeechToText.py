import speech_recognition as sr
import time
import sys 

stop_listening_func = None  
recognizer = sr.Recognizer() 
microphone = sr.Microphone()  
currently_listening_flag = False 

def speech_callback(r, audio_data):
    """
    Callback-Funktion, die aufgerufen wird, wenn Audiodaten empfangen wurden.
    Versucht, die Sprache zu erkennen und gibt den erkannten Text auf stdout aus.
    Fehler werden auf stderr ausgegeben.
    """
    try:
        text = r.recognize_google(audio_data, language="de-DE")
        print(text) 
        sys.stdout.flush() 
    except sr.UnknownValueError:
        print("Bereit...", file=sys.stderr)
        sys.stderr.flush()
    except sr.RequestError as e:
        print(f"Fehler bei der Anfrage an den Spracherkennungsdienst; {e}", file=sys.stderr)
        sys.stderr.flush()
    except Exception as e:
        print(f"Ein unerwarteter Fehler in der Callback-Funktion: {e}", file=sys.stderr)
        sys.stderr.flush()

def main():
    global stop_listening_func, recognizer, microphone, currently_listening_flag


    try:
        with microphone as source:
            recognizer.adjust_for_ambient_noise(source, duration=1.5)
    except Exception as e:
        print(f"Fehler bei der Mikrofoninitialisierung oder Kalibrierung: {e}", file=sys.stderr)
        sys.stderr.flush()
        print("Stelle sicher, dass ein Mikrofon angeschlossen ist und die notwendigen Berechtigungen erteilt wurden.", file=sys.stderr)
        sys.stderr.flush()
        return

    stop_listening_func = recognizer.listen_in_background(microphone, speech_callback, phrase_time_limit=4)
    currently_listening_flag = True

    try:
        while True:
            time.sleep(0.1)  
    except KeyboardInterrupt:
        pass 
    except Exception as e:
        print(f"Ein unerwarteter Fehler im Haupt-Thread: {e}", file=sys.stderr)
        sys.stderr.flush()
    finally:
        if stop_listening_func:
            stop_listening_func(wait_for_stop=False)  
            currently_listening_flag = False

if __name__ == "__main__":
    main()