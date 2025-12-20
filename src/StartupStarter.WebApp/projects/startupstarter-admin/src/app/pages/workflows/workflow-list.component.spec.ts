import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { WorkflowListComponent } from './workflow-list.component';
import { WorkflowService } from '../../services';
import { MatDialog } from '@angular/material/dialog';

describe('WorkflowListComponent', () => {
  let component: WorkflowListComponent;
  let fixture: ComponentFixture<WorkflowListComponent>;

  const mockWorkflowService = {
    getAll: () => of([]),
    getMyTasks: () => of([]),
    getById: () => of({}),
    approve: () => of({}),
    reject: () => of({}),
    cancel: () => of({})
  };

  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(false)
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkflowListComponent],
      providers: [
        { provide: WorkflowService, useValue: mockWorkflowService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(WorkflowListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
