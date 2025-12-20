import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { signal } from '@angular/core';
import { AdminLayoutComponent } from './admin-layout.component';
import { AuthService } from '../../../services';

describe('AdminLayoutComponent', () => {
  let component: AdminLayoutComponent;
  let fixture: ComponentFixture<AdminLayoutComponent>;

  const mockAuthService = {
    user: signal({ id: '1', firstName: 'John', lastName: 'Doe', email: 'test@test.com' }),
    hasAnyPermission: () => true,
    logout: jasmine.createSpy('logout')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminLayoutComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: mockAuthService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should toggle sidenav', () => {
    expect(component.sidenavOpen()).toBe(true);
    component.toggleSidenav();
    expect(component.sidenavOpen()).toBe(false);
  });

  it('should have filtered nav items', () => {
    expect(component.filteredNavItems().length).toBeGreaterThan(0);
  });
});
