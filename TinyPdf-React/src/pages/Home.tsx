import { CheckmarkRegular } from '@fluentui/react-icons';
import ImageCarousel, { type CarouselImageData } from '../components/ImageCarousel';
import ScreenshotData from '../data/Screenshots.js';
import StoreBadge from '../components/StoreBadge';
import Icon from '../assets/icon_150.png';

export default function Home() {

  const images: CarouselImageData[] = ScreenshotData;

  return (

    <>
      <div className="flex flex-col justify-center items-center space-y-2">
        <img src={Icon} className="w-12 h-12" />
        <h1 >TidyPdf</h1>
        <p className="text-sm  text-center">
          TidyPdf is a simple and easy to use desktop application for organizing and managing your PDF files.
        </p>
      </div>
      <div className="flex flex-col space-y-3">
        <h2 className="self-center">Key Features</h2>
        <ul className="space-y-2 text-sm">
          <li className="flex items-center space-x-2">
            <CheckmarkRegular fontSize={16} className="shrink-0 text-red-500" />
            <p ><strong className="font-medium text-black">Merge PDFs</strong> - Combine multiple PDF files into a single document.</p>
          </li>
          <li className="flex items-center space-x-2">
            <CheckmarkRegular fontSize={16} className="shrink-0 text-red-500" />
            <p ><strong className="font-medium text-black">Rearrange Pages</strong> - Easily change the order of pages in your PDF.</p>
          </li>
          <li className="flex items-center space-x-2">
            <CheckmarkRegular fontSize={16} className="shrink-0 text-red-500" />
            <p ><strong className="font-medium text-black">Add Images as PDF Pages</strong> - Convert images into PDF pages and add them to your document.</p>
          </li>
          <li className="flex items-center space-x-2">
            <CheckmarkRegular fontSize={16} className="shrink-0 text-red-500" />
            <p ><strong className="font-medium text-black">Completely Offline</strong> - Work with your PDF files without uploading them to the cloud or requiring an internet connection.</p>
          </li>
          <li className="flex items-center space-x-2">
            <CheckmarkRegular fontSize={16} className="shrink-0 text-red-500" />
            <p ><strong className="font-medium text-black">Simple Interface</strong> - Designed to make everyday PDF organization quick and easy.</p>
          </li>
        </ul>
      </div>
      <div className="flex flex-col space-y-3">
        <h2 className=" self-center">Screenshots</h2>
        <ImageCarousel images={images} />
      </div>
      <div className="flex flex-col space-y-3">
        <h2 className=" self-center">Download</h2>
        <StoreBadge className="self-center scale-50 origin-top h-15" />
      </div>
      <div className="flex flex-col space-y-3">
        <h2 className=" self-center">Privacy Policy</h2>
        <p className=" text-sm text-center">
          Please read our{' '}
          <a href="#/privacy" className="text-red-500 hover:underline">
            Privacy Policy
          </a>{' '}
          for more information.
        </p>
      </div>
    </>

  )
}