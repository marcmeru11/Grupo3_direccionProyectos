import os
import random
import shutil
from glob import glob

DATA_DIR = "Data/cropped_images"
OUTPUT_DIR = "dataset_yolo"
SPLIT = [0.7, 0.2, 0.1]  # train, val, test

# Create output directories
os.makedirs(f"{OUTPUT_DIR}/images/train", exist_ok=True)
os.makedirs(f"{OUTPUT_DIR}/images/val", exist_ok=True)
os.makedirs(f"{OUTPUT_DIR}/images/test", exist_ok=True)
os.makedirs(f"{OUTPUT_DIR}/labels/train", exist_ok=True)
os.makedirs(f"{OUTPUT_DIR}/labels/val", exist_ok=True)
os.makedirs(f"{OUTPUT_DIR}/labels/test", exist_ok=True)

images = [f for f in glob(f"{DATA_DIR}/*.png") + glob(f"{DATA_DIR}/*.jpg")] #load the images from the data directory

random.shuffle(images)
n = len(images)
train_end = int(SPLIT[0] * n)
val_end = int(SPLIT[1] * n) + train_end

splits = {
    "train": images[:train_end],
    "val": images[train_end:val_end],
    "test": images[val_end:]
} # Split the dataset

for split, files in splits.items(): # Copy images and labels to their respective directories
    for img_path in files:
        base = os.path.basename(img_path)
        lbl_path = os.path.splitext(img_path)[0] + ".txt"
        shutil.copy(img_path, f"{OUTPUT_DIR}/images/{split}/{base}")
        if os.path.exists(lbl_path):
            shutil.copy(lbl_path, f"{OUTPUT_DIR}/labels/{split}/{os.path.basename(lbl_path)}")
