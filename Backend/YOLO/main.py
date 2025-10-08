
import cv2



try:
    from ultralytics import YOLO
    print("YOLO importado correctamente.")
    modelo= "best.pt"  


    
    try:
        model = YOLO(modelo)
        imgRoute="img"
        
        
        print(f"Modelo cargado correctamente: {modelo}")

        # proccess the image
        results = model(imgRoute, conf=0.5,
        save=False,  #! TODO CAMBIAR PARA GUARDAR IMAGENES
        project="runs/detect",  # output directory
        name="predict"  # subdirectory name
    )

        #show the proccessed image
        annotated_frame = results[0].plot()  # Devuelve la imagen anotada
        cv2.imshow("YOLO Result", annotated_frame)
        cv2.waitKey(0)
        cv2.destroyAllWindows()

        # Guardar imagen con detecciones
        #results[0].save()

        
    except Exception as e:
        print(f"No se pudo cargar el modelo {modelo}: {e}")

except ImportError:
    print("YOLO no está instalado en este entorno.")



