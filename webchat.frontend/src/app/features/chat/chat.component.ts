import { TitleCasePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { AvatarService } from '../../core/services/avatar.service';
import { SessionStorageService } from '../../core/services/session-storage.service';

@Component({
  selector: 'app-chat',
  imports: [
    TitleCasePipe
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})

export class ChatComponent implements OnInit {
  private _avatarService = inject(AvatarService);
  private _sanitizer = inject(DomSanitizer);

  public sessionService = inject(SessionStorageService);
  public avatarUrl: SafeUrl | null = null;

  ngOnInit(): void {
    this.fetchAvatar(this.sessionService.get()?.avatarId as number);
  }

  fetchAvatar(id: number) {
    this._avatarService.getAvatarById(id).subscribe({
      next: (blob: Blob) => {
        const objectUrl = URL.createObjectURL(blob);
        this.avatarUrl = this._sanitizer.bypassSecurityTrustUrl(objectUrl);
      },
      error: (err) => {
        console.error('Error fetching avatar:', err);
      },
    });
  }
}