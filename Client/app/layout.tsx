import "./globals.css";
import type { Metadata } from "next";
import { Poppins } from 'next/font/google'
import { AlertProvider } from '@/providers/AlertProvider';


const poppins = Poppins({
	subsets: ['latin'],
	weight: ['100', '200', '300', '400', '500', '600', '700'],
});

export const metadata: Metadata = {
	title: "Dammen",
	authors: [{ name: "Martijn Schuman" }],
	robots: {
		index: false,
		follow: true,
		nocache: true,
		googleBot: {
		  index: true,
		  follow: false,
		  noimageindex: true,
		  'max-video-preview': -1,
		  'max-image-preview': 'large',
		  'max-snippet': -1,
		},
	  },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode,
}) {
	return (
		<html lang="nl">
			<head> 
				<link rel="shortcut icon" href="/images/favicon.ico" type="image/x-icon"/>
			</head>
			<body style={poppins.style}>	  
				<main className="h-screen" id="content">
					<AlertProvider>
						{children}
					</AlertProvider>
				</main>
				<script src="https://cdnjs.cloudflare.com/ajax/libs/flowbite/2.2.1/flowbite.min.js"></script>
			</body>
		</html>
	);
}
