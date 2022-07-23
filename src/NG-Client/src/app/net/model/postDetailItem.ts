/**
 * 后台api文档
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v1
 * 
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */
import { Accounts } from './accounts';
import { PostCategories } from './postCategories';
import { PostStatus } from './postStatus';
import { PostTagItem } from './postTagItem';

export interface PostDetailItem { 
    id?: number;
    customLink?: string;
    title?: string;
    summary?: string;
    addTime?: Date;
    account?: Accounts;
    status?: PostStatus;
    readonly statusName?: string;
    markDown?: string;
    cover?: string;
    categories?: Array<PostCategories>;
    content?: string;
    tagItems?: Array<PostTagItem>;
    readonly tags?: Array<string>;
}