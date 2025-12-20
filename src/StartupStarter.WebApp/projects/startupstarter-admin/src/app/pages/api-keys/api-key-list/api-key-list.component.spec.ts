import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiKeyListComponent } from './api-key-list.component';
import { ApiKeyService } from '../../../services';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

describe('ApiKeyListComponent', () => {
  let component: ApiKeyListComponent;
  let fixture: ComponentFixture<ApiKeyListComponent>;

  const mockApiKeyService = {
    getAll: () => of([]),
    getById: () => of({}),
    create: () => of({}),
    revoke: () => of({})
  };

  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(false)
    })
  };

  const mockSnackBar = {
    open: jasmine.createSpy('open')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApiKeyListComponent],
      providers: [
        { provide: ApiKeyService, useValue: mockApiKeyService },
        { provide: MatDialog, useValue: mockDialog },
        { provide: MatSnackBar, useValue: mockSnackBar }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ApiKeyListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
