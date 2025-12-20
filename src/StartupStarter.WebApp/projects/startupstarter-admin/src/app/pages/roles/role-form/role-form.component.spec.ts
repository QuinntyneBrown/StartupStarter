import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { RoleFormComponent } from './role-form.component';
import { RoleService } from '../../../services';

describe('RoleFormComponent', () => {
  let component: RoleFormComponent;
  let fixture: ComponentFixture<RoleFormComponent>;

  const mockRoleService = {
    getById: () => of({}),
    create: () => of({}),
    update: () => of({})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleFormComponent],
      providers: [
        provideRouter([]),
        { provide: RoleService, useValue: mockRoleService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleFormComponent);
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
    expect(component.form.get('roleName')?.value).toBe('');
    expect(component.form.get('description')?.value).toBe('');
  });

  it('should have required validators on roleName', () => {
    const control = component.form.get('roleName');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('Admin');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should not have required validators on description', () => {
    const control = component.form.get('description');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should invalidate form when roleName is empty', () => {
    component.form.patchValue({
      roleName: ''
    });
    expect(component.form.invalid).toBe(true);
  });

  it('should validate form when roleName is filled', () => {
    component.form.patchValue({
      roleName: 'Admin'
    });
    expect(component.form.valid).toBe(true);
  });

  it('should initialize selectedPermissions as empty Set', () => {
    expect(component.selectedPermissions).toBeInstanceOf(Set);
    expect(component.selectedPermissions.size).toBe(0);
  });

  it('should toggle permission selection', () => {
    const permission = 'users:read';
    expect(component.selectedPermissions.has(permission)).toBe(false);

    component.togglePermission(permission);
    expect(component.selectedPermissions.has(permission)).toBe(true);

    component.togglePermission(permission);
    expect(component.selectedPermissions.has(permission)).toBe(false);
  });
});
