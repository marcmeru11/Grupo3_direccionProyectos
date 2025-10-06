try:
    from ultralytics import YOLO
    print("[✅] YOLO importado correctamente.")

    # Intentar cargar un modelo preentrenado
    modelo_prueba = "yolov10n.pt"  # Cambia por la ruta de tus pesos
    try:
        model = YOLO(modelo_prueba)
        print(f"[✅] Modelo cargado correctamente: {modelo_prueba}")
    except Exception as e:
        print(f"[⚠️] No se pudo cargar el modelo {modelo_prueba}: {e}")

except ImportError:
    print("[❌] YOLO no está instalado en este entorno.")
