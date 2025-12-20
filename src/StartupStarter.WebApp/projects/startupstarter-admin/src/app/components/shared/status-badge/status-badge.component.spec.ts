import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StatusBadgeComponent, BadgeType } from './status-badge.component';

describe('StatusBadgeComponent', () => {
  let component: StatusBadgeComponent;
  let fixture: ComponentFixture<StatusBadgeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusBadgeComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(StatusBadgeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default empty label', () => {
    expect(component.label).toBe('');
  });

  it('should have default neutral type', () => {
    expect(component.type).toBe('neutral');
  });

  it('should set label input', () => {
    const testLabel = 'Active';
    component.label = testLabel;
    fixture.detectChanges();
    expect(component.label).toBe(testLabel);
  });

  it('should set type to success', () => {
    const testType: BadgeType = 'success';
    component.type = testType;
    fixture.detectChanges();
    expect(component.type).toBe('success');
  });

  it('should set type to warning', () => {
    const testType: BadgeType = 'warning';
    component.type = testType;
    fixture.detectChanges();
    expect(component.type).toBe('warning');
  });

  it('should set type to error', () => {
    const testType: BadgeType = 'error';
    component.type = testType;
    fixture.detectChanges();
    expect(component.type).toBe('error');
  });

  it('should set type to info', () => {
    const testType: BadgeType = 'info';
    component.type = testType;
    fixture.detectChanges();
    expect(component.type).toBe('info');
  });
});
