import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmptyStateComponent } from './empty-state.component';

describe('EmptyStateComponent', () => {
  let component: EmptyStateComponent;
  let fixture: ComponentFixture<EmptyStateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmptyStateComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(EmptyStateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default empty icon', () => {
    expect(component.icon).toBe('');
  });

  it('should have default title', () => {
    expect(component.title).toBe('No items found');
  });

  it('should have default empty message', () => {
    expect(component.message).toBe('');
  });

  it('should have default empty actionLabel', () => {
    expect(component.actionLabel).toBe('');
  });

  it('should set icon input', () => {
    const testIcon = 'inbox';
    component.icon = testIcon;
    fixture.detectChanges();
    expect(component.icon).toBe(testIcon);
  });

  it('should set title input', () => {
    const testTitle = 'No data available';
    component.title = testTitle;
    fixture.detectChanges();
    expect(component.title).toBe(testTitle);
  });

  it('should set message input', () => {
    const testMessage = 'Try adjusting your filters';
    component.message = testMessage;
    fixture.detectChanges();
    expect(component.message).toBe(testMessage);
  });

  it('should set actionLabel input', () => {
    const testActionLabel = 'Create New';
    component.actionLabel = testActionLabel;
    fixture.detectChanges();
    expect(component.actionLabel).toBe(testActionLabel);
  });

  it('should emit action event when action is triggered', () => {
    let actionEmitted = false;
    component.action.subscribe(() => {
      actionEmitted = true;
    });

    component.action.emit();
    expect(actionEmitted).toBe(true);
  });
});
