import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ImagerAppService } from '../../../core/services/imager-app.service';
import { HttpEvent, HttpEventType } from '@angular/common/http';

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

  uploadProgress = 0;
  isUploading = false;

  constructor(
    private fb: FormBuilder,
    private imagerService: ImagerAppService
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      image: [null, Validators.required]
    });
  }

  // ---------------------------------
  // FILE HANDLING
  // ---------------------------------
  onFileSelected(event: any) {
    const file = event.target.files[0];
    this.validateAndSetFile(file);
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
    this.validateAndSetFile(file || null);
  }

  validateAndSetFile(file: File | null) {
    if (!file) return;

    // Validate file type
    if (!file.type.startsWith('image/')) {
      this.form.get('image')?.setErrors({ invalidType: true })
      return;
    }

    // Optional: Validate file size (2MB max)
    if (file.size > 6 * 1024 * 1024) {
      this.form.get('image')?.setErrors({ maxSize: true })
      return;
    }

    this.uploadedFile = file;
    this.form.patchValue({ image: file });

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result;
    };
    reader.readAsDataURL(file);

  }

  clearImage() {
    this.uploadedFile = null;
    this.previewUrl = null;
    this.form.patchValue({ image: null });
    this.form.get('image')?.setErrors({ required: true });
  }

  // --------------------------------------
  // SIMULATED UPLOAD
  // --------------------------------------
  submit() {
    if (this.form.invalid || !this.uploadedFile) return;

    const formData = new FormData();
    formData.append('title', this.form.value.title);
    formData.append('description', this.form.value.description);
    formData.append('file', this.uploadedFile);

    this.isUploading = true;
    this.uploadProgress = 0;

    this.imagerService.uploadImage(formData).subscribe({
      next: (event: HttpEvent<any>) => {
        if (event.type === HttpEventType.UploadProgress && event.total) {
          this.uploadProgress = Math.round(100 * event.loaded / event.total);
        } else if (event.type === HttpEventType.Response) {
          console.log('Upload complete: ', event.body)
          this.isUploading = false;
        }
      },
      error: (err) => {
        console.error('Upload failed: ', err);
        this.isUploading = false;
      }
    });



  }

}
