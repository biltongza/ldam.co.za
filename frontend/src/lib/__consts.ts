import type { FriendSite } from './types';

export const StorageBaseUrl = 'https://cdncf.ldam.co.za/portfolio';
export const MaxDimensionNormalDensity = 320;
export const ThumbnailHrefNormalDensity = 'thumbnail2x';
export const MaxDimensionHighDensity = 640;
export const ThumbnailHrefHighDensity = '640';
export const HighResHref = '2048';
export const HighResMaxDimension = 2048;
export const DateFormat = 'D MMM YYYY';
export const FriendSites: FriendSite[] = [
  {
    domain: 'https://nabeelvalley.co.za',
    title: 'Nabeel Valley',
    name: 'Nabeel Valley',
    blurb: 'A close friend of mine, Nabeel is a wonderful photographer and a relentless improver'
  },
  {
    domain: 'https://vishen.co.za/',
    title: 'Vishen Gounden',
    name: 'Vishen Gounden',
    blurb: 'One of my favourite people I have ever had the pleasure of working with.'
  },
  {
    domain: 'https://kurtlourens.com/',
    title: 'Kurt Lourens',
    name: 'Kurt Lourens',
    blurb: "Check out Kurt's crazy No Man's Sky work!"
  },
  {
    domain: 'https://hub.blaarkies.com/',
    title: 'Pierre Roux',
    name: 'Pierre Roux',
    blurb: "I thought Pierre's KSP Visual Calculator was really cool"
  }
];
