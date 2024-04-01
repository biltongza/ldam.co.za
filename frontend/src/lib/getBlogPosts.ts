import { processMetadata } from '$lib/blog/markdown';
import dayjs from 'dayjs';
import fs from 'fs';

const mdRegex = /.+\.md$/;

export const getBlogPosts = async function () {
  const filesPromise = new Promise<string[]>((resolve, reject) => {
    fs.readdir(`src/posts`, (err, files) => {
      if (err) {
        reject(err);
      }
      resolve(files.filter((fileName) => mdRegex.test(fileName)));
    });
  });
  const files = await filesPromise;
  const postMetas = await Promise.all(
    files.map(async (fileName) => {
      const { metadata } = await processMetadata(`src/posts/${fileName}`);
      return metadata;
    })
  );

  // sort the posts by create date.
  postMetas.sort((a, b) => dayjs(b.date).valueOf() - dayjs(a.date).valueOf());

  return postMetas;
};
