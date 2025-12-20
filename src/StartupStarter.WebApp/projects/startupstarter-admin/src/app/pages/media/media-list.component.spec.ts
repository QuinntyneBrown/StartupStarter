import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { MediaListComponent } from './media-list.component';
import { MediaService } from '../../services';
import { MatDialog } from '@angular/material/dialog';

describe('MediaListComponent', () => {
  let component: MediaListComponent;
  let fixture: ComponentFixture<MediaListComponent>;

  const mockMediaService = {
    getAll: () => of([]),
    getById: () => of({}),
    upload: () => of({}),
    download: () => of(new Blob()),
    delete: () => of({})
  };

  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(false)
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MediaListComponent],
      providers: [
        { provide: MediaService, useValue: mockMediaService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MediaListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
