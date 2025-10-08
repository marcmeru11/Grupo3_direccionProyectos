from ultralytics import YOLO

model = YOLO("yolov10n.pt")

model.train(
    data="data",
    epochs=100,
    imgsz=640,
    batch=8,
    augment=True,
    degrees=5,
    translate=0.1,
    scale=0.2,
    shear=0.1,
    flipud=0.1,
    fliplr=0.5,
    mosaic=0.2,  # cuidado con mosaic si el defecto es muy pequeño
)
