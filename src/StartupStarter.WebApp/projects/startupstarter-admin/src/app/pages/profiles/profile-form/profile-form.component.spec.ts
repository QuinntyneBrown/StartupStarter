import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ProfileFormComponent } from './profile-form.component';
import { ProfileService } from '../../../services';

describe('ProfileFormComponent', () => {
  let component: ProfileFormComponent;
  let fixture: ComponentFixture<ProfileFormComponent>;

  const mockProfileService = {
    getById: () => of({}),
    create: () => of({}),
    update: () => of({})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileFormComponent],
      providers: [
        provideRouter([]),
        { provide: ProfileService, useValue: mockProfileService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileFormComponent);
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
    expect(component.form.get('profileName')?.value).toBe('');
    expect(component.form.get('profileType')?.value).toBeTruthy();
  });

  it('should have required validators on profileName', () => {
    const control = component.form.get('profileName');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('My Profile');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should invalidate form when profileName is empty', () => {
    component.form.patchValue({
      profileName: ''
    });
    expect(component.form.invalid).toBe(true);
  });

  it('should validate form when profileName is filled', () => {
    component.form.patchValue({
      profileName: 'My Profile'
    });
    expect(component.form.valid).toBe(true);
  });

  it('should have profileTypes array populated', () => {
    expect(component.profileTypes).toBeDefined();
    expect(component.profileTypes.length).toBeGreaterThan(0);
  });
});
