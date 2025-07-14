import React from 'react';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

interface StatisticsProps {
    titel: string;
    value: string;
    icon: IconProp;
    iconHeight: number;
    iconColor: string;
}

const Statistics: React.FC<StatisticsProps> = (props) => {
    return (
        <div className="content-element col-span-1 w-full md:col-span-1 shadow-lg rounded-md p-3 h-full">
            <h2 className="text-xl font-medium hidden sm:block">{props.titel}</h2>
            <FontAwesomeIcon icon={props.icon} height={props.iconHeight} className={"sm:hidden " + props.iconColor}/>

            <p className="text-xl font-medium">{props.value}</p>
        </div>
    );
};

export default Statistics;