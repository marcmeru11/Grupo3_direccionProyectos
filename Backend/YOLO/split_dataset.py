import os
import random
import shutil
from glob import glob

# DATA_DIR = "Data/cropped_images"
# OUTPUT_DIR = "dataset_yolo"
# SPLIT = [0.7, 0.2, 0.1]  # train, val, test

"""
    A utility class for splitting a dataset of images and their corresponding YOLO label files
    into training, validation, and test subsets.

    This class takes a directory of labeled images (e.g., .jpg/.png images with corresponding .txt label files)
    and divides them according to a given ratio, organizing them into YOLO-compatible folder structures.

    Attributes
    
        data_dir : str
            Path to the directory containing the source images and labels.
        output_dir : str
            Path to the directory where the split dataset will be stored.
        split : list of float
            List of three values representing the proportions for train, validation, and test splits.
"""
class split_dataset:
    
    """
        Initializes the split_dataset class with the provided parameters.

        Parameters:
        
            data_dir : str, optional
                Directory containing the original images and labels. Default is "Data/cropped_images".
            output_dir : str, optional
                Directory where the split dataset will be saved. Default is "dataset_yolo".
            split : list of float, optional
                Proportions for train, validation, and test datasets. Default is [0.7, 0.2, 0.1].
    """
    def __init__(self, data_dir="Data/cropped_images", output_dir="dataset_yolo", split=[0.7, 0.2, 0.1]):
        self.data_dir = data_dir
        self.output_dir = output_dir
        self.split = split
    
    """
        Creates the required directory structure for YOLO dataset organization.

        This includes separate folders for training, validation, and test sets,
        each containing subfolders for images and labels.
    """
    def _create_folders(self):
        os.makedirs(f"{self.output_dir}/images/train", exist_ok=True)
        os.makedirs(f"{self.output_dir}/images/val", exist_ok=True)
        os.makedirs(f"{self.output_dir}/images/test", exist_ok=True)
        os.makedirs(f"{self.output_dir}/labels/train", exist_ok=True)
        os.makedirs(f"{self.output_dir}/labels/val", exist_ok=True)
        os.makedirs(f"{self.output_dir}/labels/test", exist_ok=True)


    """
        Splits the dataset into training, validation, and test subsets
        according to the provided proportions.

        The method randomly shuffles the images, assigns them to the respective subsets,
        and copies both images and their corresponding YOLO label files (.txt)
        into the appropriate folders.

        If a label file does not exist for an image, it is skipped.
    """
    def split_data(self):
        
        images = [f for f in glob(f"{self.data_dir}/*.png") + glob(f"{self.data_dir}/*.jpg")] #load the images from the data directory

        random.shuffle(images)
        n = len(images)
        train_end = int(self.split[0] * n)
        val_end = int(self.split[1] * n) + train_end

        splits = {
            "train": images[:train_end],
            "val": images[train_end:val_end],
            "test": images[val_end:]
        } # Split the dataset

        for split, files in splits.items(): # Copy images and labels to their respective directories
            for img_path in files:
                base = os.path.basename(img_path)
                lbl_path = os.path.splitext(img_path)[0] + ".txt"
                shutil.copy(img_path, f"{self.output_dir}/images/{split}/{base}")
                if os.path.exists(lbl_path):
                    shutil.copy(lbl_path, f"{self.output_dir}/labels/{split}/{os.path.basename(lbl_path)}")


if __name__ == "__main__":
    splitter = split_dataset()
    splitter._create_folders()
    splitter.split_data()