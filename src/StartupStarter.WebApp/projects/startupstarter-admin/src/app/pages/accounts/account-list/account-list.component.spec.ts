import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { AccountListComponent } from './account-list.component';
import { AccountService } from '../../../services';
import { MatDialog } from '@angular/material/dialog';
import { Account, AccountStatus, AccountType } from '../../../models';

describe('AccountListComponent', () => {
  let component: AccountListComponent;
  let fixture: ComponentFixture<AccountListComponent>;
  let mockAccountService: jasmine.SpyObj<AccountService>;
  let mockDialog: jasmine.SpyObj<MatDialog>;

  const mockAccounts: Account[] = [
    {
      accountId: '1',
      accountName: 'Test Account 1',
      accountType: AccountType.Enterprise,
      ownerUserId: 'user-1',
      subscriptionTier: 'Premium',
      status: AccountStatus.Active,
      createdAt: new Date(),
      updatedAt: new Date()
    },
    {
      accountId: '2',
      accountName: 'Test Account 2',
      accountType: AccountType.Team,
      ownerUserId: 'user-2',
      subscriptionTier: 'Standard',
      status: AccountStatus.Suspended,
      createdAt: new Date(),
      updatedAt: new Date()
    }
  ];

  beforeEach(async () => {
    mockAccountService = jasmine.createSpyObj('AccountService', [
      'getAll',
      'suspend',
      'reactivate',
      'delete'
    ]);
    mockAccountService.getAll.and.returnValue(of(mockAccounts));

    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);

    await TestBed.configureTestingModule({
      imports: [AccountListComponent],
      providers: [
        provideRouter([]),
        { provide: AccountService, useValue: mockAccountService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AccountListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load accounts on init', () => {
    expect(mockAccountService.getAll).toHaveBeenCalled();
    expect(component.accounts().length).toBe(2);
    expect(component.isLoading()).toBe(false);
  });

  it('should filter accounts based on search query', () => {
    component.searchQuery = 'Test Account 1';
    const filtered = component.filtered();
    expect(filtered.length).toBe(1);
    expect(filtered[0].accountName).toBe('Test Account 1');
  });

  it('should return all accounts when search query is empty', () => {
    component.searchQuery = '';
    const filtered = component.filtered();
    expect(filtered.length).toBe(2);
  });

  it('should return correct status type for Active', () => {
    const statusType = component.getStatusType(AccountStatus.Active);
    expect(statusType).toBe('success');
  });

  it('should return correct status type for Suspended', () => {
    const statusType = component.getStatusType(AccountStatus.Suspended);
    expect(statusType).toBe('warning');
  });

  it('should return correct status type for Deleted', () => {
    const statusType = component.getStatusType(AccountStatus.Deleted);
    expect(statusType).toBe('error');
  });
});
