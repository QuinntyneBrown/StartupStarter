import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { SettingsComponent } from './settings.component';
import { AuthService } from '../../services';
import { MatSnackBar } from '@angular/material/snack-bar';

describe('SettingsComponent', () => {
  let component: SettingsComponent;
  let fixture: ComponentFixture<SettingsComponent>;

  const mockAuthService = {
    user: signal({
      userId: '1',
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User'
    }),
    getSessions: () => of([]),
    changePassword: () => of({}),
    enableMfa: () => of({}),
    disableMfa: () => of({}),
    revokeSession: () => of({})
  };

  const mockSnackBar = {
    open: jasmine.createSpy('open')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SettingsComponent],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: MatSnackBar, useValue: mockSnackBar }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
