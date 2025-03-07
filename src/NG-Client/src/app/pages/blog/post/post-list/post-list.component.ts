import { Component, OnInit } from '@angular/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { finalize } from 'rxjs';
import { PCropperComponent } from 'src/app/components/panda/p-cropper/p-cropper.component';
import { AdminCategoryItem, AdminPostItemResponse, PostService } from 'src/app/net';
import { BaseTableComponent } from 'src/app/shared/BaseTableComponent';
import { AuthService } from 'src/app/shared/services/auth.service';

@Component({
  selector: 'app-post-list',
  templateUrl: './post-list.component.html',
  styleUrls: ['./post-list.component.less']
})
export class PostListComponent extends BaseTableComponent implements OnInit {


  dataList: Array<AdminPostItemResponse> = []

  constructor(
    private post: PostService,
    private auth: AuthService,
    private modal: NzModalService
  ) {
    super(() => {
      this.getData();
    })
  }

  ngOnInit(): void {
    this.getData();
  }


  getData() {
    console.log('getData')
    this.loading = true
    this.post.adminPostGetListGet(this.page, this.size).pipe(finalize(() => {
      this.loading = false;
    })).subscribe(res => {
      this.total = res.total!
      this.dataList = res.data!
    })
  }

  getCate(arr: AdminCategoryItem[] | undefined) {
    if (arr) {

    }

  }


}
