import React, { useEffect, useState } from "react";
import Table from "react-bootstrap/Table";
import { Link, useParams, Navigate } from "react-router-dom";
import { getPostsFilter } from "../../../Services/BlogRepository";
import Loading from "../../../Components/Loading";
import { isInteger } from "../../../Utils/Utils";
import PostFilterPane from "../../../Components/Admin/PostFilterPane";
import { useSelector } from "react-redux";
const Posts = () => {
  const [postsList, setPostsList] = useState([]),
    [isVisibleLoading, setIsVisibleLoading] = useState(true),
    postFilter = useSelector((state) => state.postFilter);
  let { id } = useParams(),
    p = 1,
    ps = 10;
  useEffect(() => {
    document.title = "Danh sách bài viết";
    getPostsFilter(
      postFilter.keyword,
      postFilter.authorId,
      postFilter.categoryId,
      postFilter.year,
      postFilter.month,
      ps,
      p
    ).then((data) => {
      if (data) setPostsList(data.items);
      else setPostsList([]);
      setIsVisibleLoading(false);
    });
  }, [
    postFilter.keyword,
    postFilter.authorId,
    postFilter.categoryId,
    postFilter.year,
    postFilter.month,
    p,
    ps,
  ]);
};
