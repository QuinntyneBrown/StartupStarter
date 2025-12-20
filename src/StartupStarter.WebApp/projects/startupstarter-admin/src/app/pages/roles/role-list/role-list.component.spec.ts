import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { RoleListComponent } from './role-list.component';
import { RoleService } from '../../../services';
import { MatDialog } from '@angular/material/dialog';
import { Role } from '../../../models';

describe('RoleListComponent', () => {
  let component: RoleListComponent;
  let fixture: ComponentFixture<RoleListComponent>;
  let mockRoleService: jasmine.SpyObj<RoleService>;
  let mockDialog: jasmine.SpyObj<MatDialog>;

  const mockRoles: Role[] = [
    {
      roleId: '1',
      roleName: 'Administrator',
      description: 'Full system access',
      permissions: ['all'],
      isSystemRole: true,
      createdAt: new Date(),
      updatedAt: new Date()
    },
    {
      roleId: '2',
      roleName: 'Editor',
      description: 'Content management access',
      permissions: ['read', 'write'],
      isSystemRole: false,
      createdAt: new Date(),
      updatedAt: new Date()
    }
  ];

  beforeEach(async () => {
    mockRoleService = jasmine.createSpyObj('RoleService', ['getAll', 'delete']);
    mockRoleService.getAll.and.returnValue(of(mockRoles));

    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);

    await TestBed.configureTestingModule({
      imports: [RoleListComponent],
      providers: [
        provideRouter([]),
        { provide: RoleService, useValue: mockRoleService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load roles on init', () => {
    expect(mockRoleService.getAll).toHaveBeenCalled();
    expect(component.roles().length).toBe(2);
    expect(component.isLoading()).toBe(false);
  });

  it('should filter roles based on search query', () => {
    component.searchQuery = 'administrator';
    const filtered = component.filtered();
    expect(filtered.length).toBe(1);
    expect(filtered[0].roleName).toBe('Administrator');
  });

  it('should filter roles case-insensitively', () => {
    component.searchQuery = 'EDITOR';
    const filtered = component.filtered();
    expect(filtered.length).toBe(1);
    expect(filtered[0].roleName).toBe('Editor');
  });

  it('should return all roles when search query is empty', () => {
    component.searchQuery = '';
    const filtered = component.filtered();
    expect(filtered.length).toBe(2);
  });
});
