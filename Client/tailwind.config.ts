import type { Config } from "tailwindcss";

const config: Config = {
	content: [
		"./pages/**/*.{js,ts,jsx,tsx,mdx}",
		"./components/**/*.{js,ts,jsx,tsx,mdx}",
		"./app/**/*.{js,ts,jsx,tsx,mdx}",
		"./node_modules/flowbite-react/lib/**/*.js",
	],
	theme: {
		colors: {
			primary: 'rgb(var(--color-primary)) / <alpha-value>)',
			secondary: 'rgb(var(--color-secondary)) / <alpha-value>)',
		}
	},
	plugins: [
		require("flowbite/plugin")
	],
};
export default config;
