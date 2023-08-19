import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import MainMenu from './navigation/mainmenu';
import Registration from './views/loginpage/registration';
import Login from './views/loginpage/login';
import LogoutComponent from './views/loginpage/logout';
import CreateUser from './views/userpage/createuser'; 
import UpdateUser from './views/userpage/updateuser';
import UserList from './views/userpage/userlist'; 
import GetUserByEmail from './views/userpage/getuserbyemail'; 
import GetUserByNickName from './views/userpage/getuserbynickname'; 
import DeleteUser from './views/userpage/deleteuser'; 
import { MenuProvider } from './navigation/MenuContext';

const App = () => {
    return (
        <MenuProvider>
            <Router>
                <Routes>
                    <Route path="/" element={<MainMenu />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Registration />} />
                    <Route path="/logout" element={<LogoutComponent />} />
                    <Route path="/createuser" element={<CreateUser />} /> {/* Добавлен маршрут для CreateUser */}
                    <Route path="/updateuser" element={<UpdateUser />} /> {/* Добавлен маршрут для UpdateUser */}
                    <Route path="/userlist" element={<UserList />} /> 
                    <Route path="/getuserbyemail/:email" element={<GetUserByEmail />} />
                    <Route path="/getuserbynickname/:nickName" element={<GetUserByNickName />} /> 
                    <Route path="/deleteuser/:email" element={<DeleteUser />} /> 
                </Routes>
            </Router>
        </MenuProvider>
    );
};

export default App;