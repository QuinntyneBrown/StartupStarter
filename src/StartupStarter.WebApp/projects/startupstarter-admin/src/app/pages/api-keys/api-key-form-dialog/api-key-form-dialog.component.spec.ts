import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiKeyFormDialogComponent } from './api-key-form-dialog.component';
import { ApiKeyService } from '../../../services';
import { MatDialogRef } from '@angular/material/dialog';

describe('ApiKeyFormDialogComponent', () => {
  let component: ApiKeyFormDialogComponent;
  let fixture: ComponentFixture<ApiKeyFormDialogComponent>;

  const mockApiKeyService = {
    create: () => of({}),
    getAll: () => of([])
  };

  const mockDialogRef = {
    close: jasmine.createSpy('close')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApiKeyFormDialogComponent],
      providers: [
        { provide: ApiKeyService, useValue: mockApiKeyService },
        { provide: MatDialogRef, useValue: mockDialogRef }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ApiKeyFormDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
