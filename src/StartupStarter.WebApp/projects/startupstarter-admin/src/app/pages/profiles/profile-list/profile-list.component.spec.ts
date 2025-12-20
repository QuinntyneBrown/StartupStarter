import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ProfileListComponent } from './profile-list.component';
import { ProfileService } from '../../../services';
import { MatDialog } from '@angular/material/dialog';

describe('ProfileListComponent', () => {
  let component: ProfileListComponent;
  let fixture: ComponentFixture<ProfileListComponent>;

  const mockProfileService = {
    getAll: () => of([]),
    delete: () => of({}),
    setAsDefault: () => of({})
  };

  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(false)
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileListComponent],
      providers: [
        provideRouter([]),
        { provide: ProfileService, useValue: mockProfileService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with empty profiles', () => {
    expect(component.profiles()).toEqual([]);
  });

  it('should initialize isLoading as false after ngOnInit', () => {
    // After ngOnInit completes and the observable resolves
    expect(component.isLoading()).toBe(false);
  });

  it('should filter profiles based on search query', () => {
    const mockProfiles = [
      { profileId: '1', profileName: 'Personal Profile', profileType: 'Personal', isDefault: true },
      { profileId: '2', profileName: 'Business Profile', profileType: 'Business', isDefault: false }
    ];
    component.profiles.set(mockProfiles);

    component.searchQuery = 'personal';
    expect(component.filtered().length).toBe(1);
    expect(component.filtered()[0].profileName).toBe('Personal Profile');

    component.searchQuery = 'business';
    expect(component.filtered().length).toBe(1);
    expect(component.filtered()[0].profileName).toBe('Business Profile');

    component.searchQuery = '';
    expect(component.filtered().length).toBe(2);
  });

  it('should return empty array when no profiles match search', () => {
    const mockProfiles = [
      { profileId: '1', profileName: 'Personal Profile', profileType: 'Personal', isDefault: true }
    ];
    component.profiles.set(mockProfiles);

    component.searchQuery = 'nonexistent';
    expect(component.filtered().length).toBe(0);
  });

  it('should perform case-insensitive search', () => {
    const mockProfiles = [
      { profileId: '1', profileName: 'Personal Profile', profileType: 'Personal', isDefault: true }
    ];
    component.profiles.set(mockProfiles);

    component.searchQuery = 'PERSONAL';
    expect(component.filtered().length).toBe(1);

    component.searchQuery = 'personal';
    expect(component.filtered().length).toBe(1);

    component.searchQuery = 'PeRsOnAl';
    expect(component.filtered().length).toBe(1);
  });
});
