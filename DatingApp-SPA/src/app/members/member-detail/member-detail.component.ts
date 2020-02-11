import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/User';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  user: User;

  constructor(private userService: UserService, private alertity: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.LoadUser();
  }

  LoadUser() {
    this.userService.getUser(+this.route.snapshot.params.id).subscribe((user: User) => {
      this.user = user;
    }, error => {
      return this.alertity.error(error);
    });
  }

}
