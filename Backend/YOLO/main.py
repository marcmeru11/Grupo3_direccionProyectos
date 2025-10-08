
import cv2



try:
    from ultralytics import YOLO
    print("YOLO importado correctamente.")
    modelo= "best.pt"  


    
    try:
        model = YOLO(modelo)
        imgRoute="/home/david/3_carrera/dirProyectos/Grupo3_direccionProyectos/Backend/YOLO/img_2024-12-03_08.47.57_11150774_cam_H2674069.png"
        # Cargar modelo entrenado
        
        print(f"Modelo cargado correctamente: {modelo}")

        # Detectar objetos en una imagen
        results = model(imgRoute, conf=0.5,
        save=False,  #! TODO CAMBIAR PARA GUARDAR IMAGENES
        project="runs/detect",  # Carpeta de salida
        name="predict2"  # Subcarpeta
    )

        # Mostrar resultados
        annotated_frame = results[0].plot()  # Devuelve la imagen anotada
        cv2.imshow("YOLO Result", annotated_frame)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

        # Guardar imagen con detecciones
        #results[0].save()

        # Imprimir resultados en consola
        print(results)
    except Exception as e:
        print(f"No se pudo cargar el modelo {modelo}: {e}")

except ImportError:
    print("YOLO no está instalado en este entorno.")



