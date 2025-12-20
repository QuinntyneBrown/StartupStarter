import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { UserListComponent } from './user-list.component';
import { UserService } from '../../../services';
import { MatDialog } from '@angular/material/dialog';
import { User, UserStatus } from '../../../models';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockDialog: jasmine.SpyObj<MatDialog>;

  const mockUsers: User[] = [
    {
      userId: '1',
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      accountId: 'account-1',
      status: UserStatus.Active,
      createdAt: new Date(),
      updatedAt: new Date()
    },
    {
      userId: '2',
      firstName: 'Jane',
      lastName: 'Smith',
      email: 'jane.smith@example.com',
      accountId: 'account-1',
      status: UserStatus.Inactive,
      createdAt: new Date(),
      updatedAt: new Date()
    }
  ];

  beforeEach(async () => {
    mockUserService = jasmine.createSpyObj('UserService', [
      'getAll',
      'activate',
      'deactivate',
      'lock',
      'unlock',
      'delete'
    ]);
    mockUserService.getAll.and.returnValue(of(mockUsers));

    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);

    await TestBed.configureTestingModule({
      imports: [UserListComponent],
      providers: [
        provideRouter([]),
        { provide: UserService, useValue: mockUserService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load users on init', () => {
    expect(mockUserService.getAll).toHaveBeenCalled();
    expect(component.users().length).toBe(2);
    expect(component.isLoading()).toBe(false);
  });

  it('should filter users by first name', () => {
    component.searchQuery = 'john';
    const filtered = component.filtered();
    expect(filtered.length).toBe(1);
    expect(filtered[0].firstName).toBe('John');
  });

  it('should filter users by last name', () => {
    component.searchQuery = 'smith';
    const filtered = component.filtered();
    expect(filtered.length).toBe(1);
    expect(filtered[0].lastName).toBe('Smith');
  });

  it('should filter users by email', () => {
    component.searchQuery = 'jane.smith';
    const filtered = component.filtered();
    expect(filtered.length).toBe(1);
    expect(filtered[0].email).toBe('jane.smith@example.com');
  });

  it('should return all users when search query is empty', () => {
    component.searchQuery = '';
    const filtered = component.filtered();
    expect(filtered.length).toBe(2);
  });

  it('should return correct status type for Active', () => {
    const statusType = component.getStatusType(UserStatus.Active);
    expect(statusType).toBe('success');
  });

  it('should return correct status type for Inactive', () => {
    const statusType = component.getStatusType(UserStatus.Inactive);
    expect(statusType).toBe('neutral');
  });

  it('should return correct status type for Locked', () => {
    const statusType = component.getStatusType(UserStatus.Locked);
    expect(statusType).toBe('error');
  });

  it('should return correct status type for Invited', () => {
    const statusType = component.getStatusType(UserStatus.Invited);
    expect(statusType).toBe('info');
  });

  it('should return correct status type for Deleted', () => {
    const statusType = component.getStatusType(UserStatus.Deleted);
    expect(statusType).toBe('error');
  });
});
