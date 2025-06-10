import cv2
import time
import ctypes
import sys

# --- Globale Variablen ---
CHECK_INTERVAL = 0.5
last_face_detected_time = time.time()
is_locked_flag = False

HAAR_CASCADE_FRONTAL_PATH = cv2.data.haarcascades + 'haarcascade_frontalface_default.xml'
HAAR_CASCADE_PROFILE_PATH = cv2.data.haarcascades + 'haarcascade_profileface.xml'

def lock_workstation():
    global is_locked_flag
    if not is_locked_flag:
        try:
            ctypes.windll.user32.LockWorkStation()
            print("STATUS: PC_LOCKED")
            sys.stdout.flush()
            is_locked_flag = True
        except Exception as e:
            print(f"ERROR: Fehler beim Sperren des PCs: {e}", file=sys.stderr)
            sys.stderr.flush()

def main(seconds_to_lock_arg):
    global last_face_detected_time
    global is_locked_flag

    try:
        current_seconds_to_lock = int(seconds_to_lock_arg)
        if current_seconds_to_lock <= 0:
            current_seconds_to_lock = 10
    except ValueError:
        current_seconds_to_lock = 10
    
    print(f"INFO: Headless-Überwachung gestartet. seconds_to_lock = {current_seconds_to_lock}")
    sys.stdout.flush()

    frontal_face_cascade = cv2.CascadeClassifier(HAAR_CASCADE_FRONTAL_PATH)
    profile_face_cascade = cv2.CascadeClassifier(HAAR_CASCADE_PROFILE_PATH)

    if frontal_face_cascade.empty():
        print(f"ERROR: Konnte frontale Haar-Kaskade nicht laden.", file=sys.stderr)
        sys.stderr.flush()
        return
    if profile_face_cascade.empty():
        print(f"ERROR: Konnte Profil Haar-Kaskade nicht laden.", file=sys.stderr)
        sys.stderr.flush()
        return

    video_capture = cv2.VideoCapture(0)
    if not video_capture.isOpened():
        print("ERROR: Konnte die Webcam nicht öffnen.", file=sys.stderr)
        sys.stderr.flush()
        return

    last_check_time = time.time()
    no_face_counter = 0
    MAX_NO_FACE_CHECKS = current_seconds_to_lock / CHECK_INTERVAL if CHECK_INTERVAL > 0 else float('inf')

    running = True
    try:
        while running:
            current_time = time.time()
            
            time.sleep(0.01) 

            ret, frame = video_capture.read()
            if not ret:
                print("WARNING: Frame konnte nicht gelesen werden.", file=sys.stderr)
                sys.stderr.flush()
                time.sleep(0.1) 
                continue

            if current_time - last_check_time < CHECK_INTERVAL:
                continue 
            
            last_check_time = current_time
            gray_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            gray_frame = cv2.equalizeHist(gray_frame)

            frontal_faces = frontal_face_cascade.detectMultiScale(
                gray_frame, 
                scaleFactor=1.1, 
                minNeighbors=5, 
                minSize=(30, 30)
            )

            profile_faces = [] 
            if len(frontal_faces) == 0:
                profile_faces_left = profile_face_cascade.detectMultiScale(
                    gray_frame, 
                    scaleFactor=1.1, 
                    minNeighbors=5, 
                    minSize=(40, 40)
                )
                flipped_gray_frame = cv2.flip(gray_frame, 1)
                profile_faces_right_flipped = profile_face_cascade.detectMultiScale(
                    flipped_gray_frame, 
                    scaleFactor=1.1, 
                    minNeighbors=5, 
                    minSize=(40, 40)
                )
                profile_faces_right = []
                for (x, y, w, h) in profile_faces_right_flipped:
                    profile_faces_right.append((frame.shape[1] - x - w, y, w, h))
                profile_faces = list(profile_faces_left) + list(profile_faces_right)


            total_faces_found = len(frontal_faces) + len(profile_faces)

            if total_faces_found > 0:
                last_face_detected_time = current_time
                no_face_counter = 0
                is_locked_flag = False
            else:
                no_face_counter += 1
                if no_face_counter >= MAX_NO_FACE_CHECKS:
                    if current_time - last_face_detected_time > current_seconds_to_lock:
                        lock_workstation()
            
    except KeyboardInterrupt: 
        print("INFO: KeyboardInterrupt erhalten, beende Überwachung.")
        sys.stdout.flush()
    except SystemExit: 
        print("INFO: SystemExit erhalten, beende Überwachung.")
        sys.stdout.flush()
    except Exception as e:
        print(f"ERROR: Unerwarteter Fehler in der Hauptschleife: {e}", file=sys.stderr)
        sys.stderr.flush()
    finally:
        video_capture.release()
        print("INFO: Überwachung (headless) beendet und Ressourcen freigegeben.")
        sys.stdout.flush()

if __name__ == "__main__":
    if len(sys.argv) > 1:
        seconds_param = sys.argv[1]
    else:
        seconds_param = "10"
        print(f"WARNING: Kein seconds_to_lock Argument, verwende Standard: {seconds_param}s", file=sys.stderr)
        sys.stderr.flush()
    main(seconds_param)