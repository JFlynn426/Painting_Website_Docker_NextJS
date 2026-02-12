'use client'

import Image from 'next/image';
import { Carousel } from 'react-bootstrap';

export default function ArtCarousel() {
    return (
        <Carousel fade={true} controls={true} indicators={true}>
            <Carousel.Item>
                <div className="d-flex justify-content-center align-items-center" style={{ minHeight: "75vh" }}>
                    <Image
                        src="/Cloud Creatures23crop.jpg"
                        alt="Cloud Creatures"
                        width={800}
                        height={600}
                        className="d-block h-auto rounded-lg shadow-xl"
                        style={{ maxHeight: "75vh", objectFit: "contain" }}
                        priority={true}
                    />
                </div>
            </Carousel.Item>
            <Carousel.Item>
                <div className="d-flex justify-content-center align-items-center" style={{ minHeight: "75vh" }}>
                    <Image
                        src="/Turtle_Painting23crop.jpg"
                        alt="Turtle Painting"
                        width={800}
                        height={600}
                        className="d-block h-auto rounded-lg shadow-xl"
                        style={{ maxHeight: "75vh", objectFit: "contain" }}
                        priority={true}
                    />
                </div>
            </Carousel.Item>
            <Carousel.Item>
                <div className="d-flex justify-content-center align-items-center" style={{ minHeight: "75vh" }}>
                    <Image
                        src="/VioletCuryPreserve.jpg"
                        alt="Violet Cury Preserve"
                        width={800}
                        height={600}
                        className="d-block h-auto rounded-lg shadow-xl"
                        style={{ maxHeight: "75vh", objectFit: "contain" }}
                        priority={true}
                    />
                </div>
            </Carousel.Item>
        </Carousel>
    );
}