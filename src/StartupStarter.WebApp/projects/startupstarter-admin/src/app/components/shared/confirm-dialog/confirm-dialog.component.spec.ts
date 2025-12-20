import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConfirmDialogComponent, ConfirmDialogData } from './confirm-dialog.component';

describe('ConfirmDialogComponent', () => {
  let component: ConfirmDialogComponent;
  let fixture: ComponentFixture<ConfirmDialogComponent>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<ConfirmDialogComponent>>;
  let mockDialogData: ConfirmDialogData;

  beforeEach(async () => {
    mockDialogRef = jasmine.createSpyObj('MatDialogRef', ['close']);
    mockDialogData = {
      title: 'Test Title',
      message: 'Test Message',
      confirmText: 'Yes',
      cancelText: 'No',
      confirmColor: 'primary'
    };

    await TestBed.configureTestingModule({
      imports: [ConfirmDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MAT_DIALOG_DATA, useValue: mockDialogData }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should inject dialog data', () => {
    expect(component.data).toBe(mockDialogData);
    expect(component.data.title).toBe('Test Title');
    expect(component.data.message).toBe('Test Message');
    expect(component.data.confirmText).toBe('Yes');
    expect(component.data.cancelText).toBe('No');
    expect(component.data.confirmColor).toBe('primary');
  });

  it('should close dialog with true when onConfirm is called', () => {
    component.onConfirm();
    expect(mockDialogRef.close).toHaveBeenCalledWith(true);
  });

  it('should close dialog with false when onCancel is called', () => {
    component.onCancel();
    expect(mockDialogRef.close).toHaveBeenCalledWith(false);
  });

  it('should handle dialog data with optional properties', () => {
    const minimalData: ConfirmDialogData = {
      title: 'Minimal Title',
      message: 'Minimal Message'
    };

    TestBed.resetTestingModule();
    const mockDialogRef2 = jasmine.createSpyObj('MatDialogRef', ['close']);

    TestBed.configureTestingModule({
      imports: [ConfirmDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef2 },
        { provide: MAT_DIALOG_DATA, useValue: minimalData }
      ]
    });

    const fixture2 = TestBed.createComponent(ConfirmDialogComponent);
    const component2 = fixture2.componentInstance;

    expect(component2.data.title).toBe('Minimal Title');
    expect(component2.data.message).toBe('Minimal Message');
    expect(component2.data.confirmText).toBeUndefined();
    expect(component2.data.cancelText).toBeUndefined();
    expect(component2.data.confirmColor).toBeUndefined();
  });
});
