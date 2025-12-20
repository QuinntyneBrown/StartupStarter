import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { signal } from '@angular/core';
import { DashboardComponent } from './dashboard.component';
import { AuthService } from '../../services';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  const mockAuthService = {
    user: signal({ firstName: 'Test', lastName: 'User', email: 'test@example.com' })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: mockAuthService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set userName from AuthService on init', () => {
    expect(component.userName()).toBe('Test');
  });

  it('should initialize with loading state', () => {
    expect(component.isLoading()).toBe(true);
  });

  it('should have initial dashboard cards', () => {
    const cards = component.cards();
    expect(cards.length).toBe(4);
    expect(cards[0].title).toBe('Total Users');
    expect(cards[1].title).toBe('Active Content');
    expect(cards[2].title).toBe('Pending Approvals');
    expect(cards[3].title).toBe('Media Files');
  });

  it('should have recent activity items', () => {
    const activities = component.recentActivity();
    expect(activities.length).toBe(5);
  });
});
