import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-upload-form',
  templateUrl: './upload-form.component.html',
  styleUrl: './upload-form.component.css'
})
export class UploadFormComponent {
  form: FormGroup;
  uploadedFile: File | null = null;
  previewUrl: string | ArrayBuffer | null = null;
  isDragging = false;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      image: [null, Validators.required]
    });
  }

  setFile(file: File | null) {
    if (file) {
      this.uploadedFile = file;
      this.form.patchValue({ image: file });
      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    this.setFile(file);
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave() {
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
    const file = event.dataTransfer?.files[0];
    this.setFile(file || null);
  }

  submit() {
    if (this.form.valid) {
      console.log('Form Data: ', this.form.value);
      console.log('Uploaded File: ', this.uploadedFile);
    }
  }

}
