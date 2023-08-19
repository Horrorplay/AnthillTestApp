import React from 'react';
import { Link } from 'react-router-dom';
import { useMenu } from './MenuContext';

const MainMenu = () => {
    const { showMenu } = useMenu();

    return (
        showMenu && (
            <nav className="flex justify-center bg-gray-800">
                <ul className="flex space-x-4">
                    <li>
                        <Link to="/login" className="text-white hover:text-gray-300" target="_blank">Login</Link>
                    </li>
                    <li>
                        <Link to="/register" className="text-white hover:text-gray-300" target="_blank">Register</Link>
                    </li>
                    <li>
                        <Link to="/logout" className="text-white hover:text-gray-300">Logout</Link>
                    </li>
                    <li>
                        <Link to="/createuser" className="text-white hover:text-gray-300" target="_blank">Create User</Link>
                    </li>
                    <li>
                        <Link to="/updateuser" className="text-white hover:text-gray-300" target="_blank">Update User</Link>
                    </li>
                    <li>
                        <Link to="/getuserbynickname/:nickName" className="text-white hover:text-gray-300" target="_blank">Get User By Name</Link>
                    </li>
                    <li>
                        <Link to="/getuserbyemail/:email" className="text-white hover:text-gray-300" target="_blank">Get User By Email</Link>
                    </li>
                    <li>
                        <Link to="/userlist" className="text-white hover:text-gray-300" target="_blank">Get All Users</Link>
                    </li>
                    <li>
                        <Link to="/deleteuser/:email" className="text-white hover:text-gray-300" target="_blank">Delete User</Link>
                    </li>
                </ul>
            </nav>
        )
    );
};

export default MainMenu;