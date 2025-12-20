import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoadingSpinnerComponent } from './loading-spinner.component';

describe('LoadingSpinnerComponent', () => {
  let component: LoadingSpinnerComponent;
  let fixture: ComponentFixture<LoadingSpinnerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoadingSpinnerComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(LoadingSpinnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default diameter of 48', () => {
    expect(component.diameter).toBe(48);
  });

  it('should have default empty message', () => {
    expect(component.message).toBe('');
  });

  it('should set diameter input', () => {
    const testDiameter = 64;
    component.diameter = testDiameter;
    fixture.detectChanges();
    expect(component.diameter).toBe(testDiameter);
  });

  it('should set message input', () => {
    const testMessage = 'Loading data...';
    component.message = testMessage;
    fixture.detectChanges();
    expect(component.message).toBe(testMessage);
  });
});
