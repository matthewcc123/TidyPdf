import { ChevronLeftRegular, ChevronRightRegular, AddRegular } from "@fluentui/react-icons";
import { useEffect, useState } from "react";

export interface CarouselImageData {
    src: string;
    alt: string;
}

interface ImgCarouselProps {
    images: CarouselImageData[];
}

export default function ImgCarousel({ images }: ImgCarouselProps) {
    const [currentIndex, setCurrentIndex] = useState(0);
    const [isOpen, setIsOpen] = useState(false);
    const [isModalMounted, setIsModalMounted] = useState(false);
    const [direction, setDirection] = useState<"next" | "prev" | null>(null);

    // Open modal with transition
    const openModal = (index: number) => {
        setCurrentIndex(index);
        setDirection(null);
        setIsModalMounted(true);

        setTimeout(() => setIsOpen(true), 10);
        document.body.style.overflow = "hidden";
    };

    // Close modal with transition
    const closeModal = () => {
        setIsOpen(false);
        setTimeout(() => {
            setIsModalMounted(false);
            setDirection(null);
        }, 200);
        document.body.style.overflow = "auto";
    };

    const nextImage = () => {
        setDirection("next");
        setCurrentIndex((prevIndex) => (prevIndex + 1) % images.length);
    };

    const prevImage = () => {
        setDirection("prev");
        setCurrentIndex((prevIndex) => (prevIndex - 1 + images.length) % images.length);
    };

    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (!isModalMounted) return;
            if (e.key === "Escape") closeModal();
            if (e.key === "ArrowRight") nextImage();
            if (e.key === "ArrowLeft") prevImage();
        };
        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [isModalMounted, images.length]);

    if (!images || images.length === 0) return null;

    const currentImage = images[currentIndex];

    return (
        <>

            <style>{`
                @keyframes slideInFromRight {
                    0% { transform: translateX(100%); opacity: 0.4; }
                    100% { transform: translateX(0); opacity: 1; }
                }
                @keyframes slideInFromLeft {
                    0% { transform: translateX(-100%); opacity: 0.4; }
                    100% { transform: translateX(0); opacity: 1; }
                }
                .animate-slide-next {
                    animation: slideInFromRight 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards;
                }
                .animate-slide-prev {
                    animation: slideInFromLeft 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards;
                }
            `}</style>

            <div className="flex items-center relative w-full min-h-32 max-h-64 bg-gray-200/50 rounded-md overflow-hidden p-2 border border-gray-300/50">

                <div
                    className="h-full flex transition-transform duration-300 ease-in-out"
                    style={{ transform: `translateX(calc(-${currentIndex * 60}% + 10%))` }}
                >
                    {images.map((image, index) => (
                        <div
                            key={index}
                            className="shrink-0 w-[60%] px-2 flex justify-center items-center hover:scale-102 transition-transform duration-300"
                            onClick={() => openModal(index)}
                        >
                            <img
                                src={image.src}
                                alt={image.alt}
                                className="cursor-pointer max-h-full max-w-full rounded-md object-cover"
                            />
                        </div>
                    ))}
                </div>

                <button
                    onClick={prevImage}
                    className="cursor-pointer shadow-md absolute top-1/2 left-2 bg-white/70 text-black rounded-full p-1 flex items-center justify-center transform -translate-y-1/2 hover:bg-white border border-gray-300/50 transition-colors duration-100 z-10"
                >
                    <ChevronLeftRegular fontSize={16} className="shrink-0" />
                </button>

                <button
                    onClick={nextImage}
                    className="cursor-pointer shadow-md absolute top-1/2 right-2 bg-white/70 text-black rounded-full p-1 flex items-center justify-center transform -translate-y-1/2 hover:bg-white border border-gray-300/50 transition-colors duration-100 z-10"
                >
                    <ChevronRightRegular fontSize={16} className="shrink-0" />
                </button>


                {isModalMounted && (
                    <div
                        className={`fixed inset-0 z-50 flex items-center justify-center bg-black/80 backdrop-blur-sm p-4 transition-opacity duration-200 ease-out ${isOpen ? "opacity-100" : "opacity-0"
                            }`}
                        onClick={closeModal}
                    >
                        <button
                            onClick={closeModal}
                            className="absolute top-4 right-4 flex items-center justify-center w-10 h-10 rounded-md bg-gray-300/10 text-white hover:text-gray-300 hover:bg-gray-300/20 z-20 transition-colors duration-100 cursor-pointer"
                        >
                            <AddRegular fontSize={24} className="shrink-0 rotate-45" />
                        </button>

                        <div
                            className={`relative max-w-5xl w-full max-h-[90vh] flex flex-col items-center justify-center transition-transform duration-200 ease-out ${isOpen ? "scale-100 opacity-100" : "scale-95 opacity-0"
                                }`}
                            onClick={(e) => e.stopPropagation()}
                        >

                            <div className="relative w-full flex flex-col items-center justify-center overflow-hidden">
                                <div
                                    key={currentIndex}
                                    className={`flex flex-col items-center justify-center max-w-full ${direction === "next"
                                        ? "animate-slide-next"
                                        : direction === "prev"
                                            ? "animate-slide-prev"
                                            : ""
                                        }`}
                                >
                                    <img
                                        src={currentImage.src}
                                        alt={currentImage.alt}
                                        className="mt-4 max-w-full max-h-[80vh] object-contain rounded-lg shadow-2xl select-none"
                                    />
                                    {currentImage.alt && (
                                        <p className="mt-4 text-white/90 text-sm text-center">
                                            {currentImage.alt}
                                        </p>
                                    )}
                                </div>
                            </div>


                            <button
                                onClick={prevImage}
                                className="cursor-pointer shadow-md absolute top-1/2 left-2 bg-white/70 hover:bg-white text-black rounded-full p-2 flex items-center justify-center transform -translate-y-1/2 border border-gray-300/50 transition-colors duration-100 z-10"
                            >
                                <ChevronLeftRegular fontSize={20} className="shrink-0" />
                            </button>

                            <button
                                onClick={nextImage}
                                className="cursor-pointer shadow-md absolute top-1/2 right-2 bg-white/70 hover:bg-white text-black rounded-full p-2 flex items-center justify-center transform -translate-y-1/2 border border-gray-300/50 transition-colors duration-100 z-10"
                            >
                                <ChevronRightRegular fontSize={20} className="shrink-0" />
                            </button>
                        </div>
                    </div>
                )}

            </div>
        </>
    );
}
