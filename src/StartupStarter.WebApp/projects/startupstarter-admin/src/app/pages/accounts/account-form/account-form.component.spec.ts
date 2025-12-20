import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { AccountFormComponent } from './account-form.component';
import { AccountService } from '../../../services';

describe('AccountFormComponent', () => {
  let component: AccountFormComponent;
  let fixture: ComponentFixture<AccountFormComponent>;

  const mockAccountService = {
    getById: () => of({}),
    create: () => of({}),
    update: () => of({})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountFormComponent],
      providers: [
        provideRouter([]),
        { provide: AccountService, useValue: mockAccountService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AccountFormComponent);
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
    expect(component.form.get('accountName')?.value).toBe('');
    expect(component.form.get('accountType')?.value).toBeTruthy();
    expect(component.form.get('ownerUserId')?.value).toBe('');
    expect(component.form.get('subscriptionTier')?.value).toBe('Free');
  });

  it('should have required validators on accountName', () => {
    const control = component.form.get('accountName');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('Test Account');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should have required validators on ownerUserId', () => {
    const control = component.form.get('ownerUserId');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('user-123');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should have required validators on subscriptionTier', () => {
    const control = component.form.get('subscriptionTier');
    control?.setValue('');
    expect(control?.hasError('required')).toBe(true);
    control?.setValue('Premium');
    expect(control?.hasError('required')).toBe(false);
  });

  it('should invalidate form when required fields are empty', () => {
    component.form.patchValue({
      accountName: '',
      ownerUserId: '',
      subscriptionTier: ''
    });
    expect(component.form.invalid).toBe(true);
  });

  it('should validate form when all required fields are filled', () => {
    component.form.patchValue({
      accountName: 'Test Account',
      ownerUserId: 'user-123',
      subscriptionTier: 'Free'
    });
    expect(component.form.valid).toBe(true);
  });
});
