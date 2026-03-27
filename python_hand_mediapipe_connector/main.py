import cv2
import mediapipe as mp
import socket
import json
import time

# ------------------ UDP CONFIG ------------------
UDP_IP = "127.0.0.1"
UDP_PORT = 5055

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# ------------------ MEDIAPIPE INIT ------------------
mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

hands = mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=1,
    model_complexity=0,
    min_detection_confidence=0.7,
    min_tracking_confidence=0.5
)

# ------------------ CAMERA ------------------
cap = cv2.VideoCapture(0)

if not cap.isOpened():
    print("ERROR: Cannot open webcam")
    exit()

print("Webcam started. Press ESC to exit.")

# ------------------ MAIN LOOP ------------------
prev_time = time.time()

while True:
    ret, frame = cap.read()
    if not ret:
        continue

    # зеркалим картинку (как в зеркале)
    frame = cv2.flip(frame, 1)

    # BGR -> RGB
    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    # обработка
    results = hands.process(rgb)

    # дефолтные значения
    data = {
        "x": 0.5,
        "y": 0.5,
        "gesture": "none"
    }

    if results.multi_hand_landmarks:
        hand_landmarks = results.multi_hand_landmarks[0]

        # рисуем скелет
        mp_drawing.draw_landmarks(
            frame,
            hand_landmarks,
            mp_hands.HAND_CONNECTIONS
        )

        # -------- координаты указательного пальца --------
        index_tip = hand_landmarks.landmark[8]
        index_pip = hand_landmarks.landmark[6]

        data["x"] = float(index_tip.x)
        data["y"] = float(index_tip.y)

        # -------- детекция жеста --------
        # простой вариант:
        # если указательный палец "вверх" относительно сустава → one_finger
        if index_tip.y < index_pip.y:
            gesture = "one_finger"
        else:
            gesture = "open"

        data["gesture"] = gesture

        # визуализация точки
        h, w, _ = frame.shape
        cx, cy = int(index_tip.x * w), int(index_tip.y * h)
        cv2.circle(frame, (cx, cy), 10, (0, 255, 0), -1)

    # -------- отправка --------
    try:
        print(data)
        message = json.dumps(data).encode("utf-8")
        sock.sendto(message, (UDP_IP, UDP_PORT))
    except Exception as e:
        print("UDP send error:", e)

    # -------- FPS --------
    current_time = time.time()
    fps = 1 / (current_time - prev_time)
    prev_time = current_time

    cv2.putText(frame, f"FPS: {int(fps)}", (10, 30),
                cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 255), 2)

    cv2.putText(frame, f"Gesture: {data['gesture']}", (10, 70),
                cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 2)

    # -------- отображение --------
    cv2.imshow("Hand Tracking UDP Sender", frame)

    # выход по ESC
    if cv2.waitKey(1) & 0xFF == 27:
        break

# ------------------ CLEANUP ------------------
cap.release()
cv2.destroyAllWindows()
sock.close()