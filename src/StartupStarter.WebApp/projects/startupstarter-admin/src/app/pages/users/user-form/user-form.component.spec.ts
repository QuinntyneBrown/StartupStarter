import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { UserFormComponent } from './user-form.component';
import { UserService, RoleService } from '../../../services';

describe('UserFormComponent', () => {
  let component: UserFormComponent;
  let fixture: ComponentFixture<UserFormComponent>;

  const mockUserService = {
    getById: () => of({}),
    update: () => of({}),
    invite: () => of({})
  };

  const mockRoleService = {
    getAll: () => of([])
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserFormComponent],
      providers: [
        provideRouter([]),
        { provide: UserService, useValue: mockUserService },
        { provide: RoleService, useValue: mockRoleService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start in create mode', () => {
    expect(component.isEditMode()).toBe(false);
  });

  it('should initialize form with default values', () => {
    expect(component.form.get('email')?.value).toBe('');
    expect(component.form.get('firstName')?.value).toBe('');
    expect(component.form.get('lastName')?.value).toBe('');
    expect(component.form.get('roleIds')?.value).toEqual([]);
  });

  it('should have required validators on email', () => {
    const control = component.form.get('email');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('test@example.com');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should have email validator on email field', () => {
    const control = component.form.get('email');
    control?.setValue('invalid-email');
    expect(control?.hasError('email')).toBe(true);
    control?.setValue('valid@email.com');
    expect(control?.hasError('email')).toBe(false);
  });

  it('should have required validators on firstName', () => {
    const control = component.form.get('firstName');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('John');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should have required validators on lastName', () => {
    const control = component.form.get('lastName');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('Doe');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should invalidate form when required fields are empty', () => {
    component.form.patchValue({
      email: '',
      firstName: '',
      lastName: ''
    });
    expect(component.form.invalid).toBe(true);
  });

  it('should validate form when all required fields are filled', () => {
    component.form.patchValue({
      email: 'test@example.com',
      firstName: 'John',
      lastName: 'Doe'
    });
    expect(component.form.valid).toBe(true);
  });
});
