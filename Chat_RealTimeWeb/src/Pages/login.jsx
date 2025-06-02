// src/pages/Login.jsx
import React, { useEffect } from "react";
import { Form, Input, Button, Alert } from "antd";
import { useDispatch, useSelector } from "react-redux";

// import { loginSuccess } from "../Features/authSlice";
import { loginByGoogle, loginUser } from "../Features/authApi";
import { useNavigate } from "react-router-dom";
import { GoogleLogin } from "@react-oauth/google";

const Login = () => {
  const dispatch = useDispatch();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { loading, error, token } = useSelector((state) => state.auth);

  const onFinish = async () => {
    const values = await form.getFieldsValue();
    const resultAction = await dispatch(loginUser(values));
    if (loginUser.fulfilled.match(resultAction)) {
      navigate("/");
    }
  };

  useEffect(() => {
    if (!token) {
      navigate("/login");
    }
  }, [token]);

  return (
    <Form form={form} style={{ maxWidth: 400, margin: "40px auto" }}>
      {error && <Alert message={error} type="error" showIcon />}
      <Form.Item
        name="email"
        rules={[{ required: true, message: "Vui lòng nhập email!" }]}
      >
        <Input placeholder="Email" />
      </Form.Item>
      <Form.Item
        name="password"
        rules={[{ required: true, message: "Vui lòng nhập mật khẩu!" }]}
      >
        <Input.Password placeholder="Mật khẩu" />
      </Form.Item>
      <Form.Item>
        <Button
          type="primary"
          onClick={onFinish}
          htmlType="submit"
          loading={loading}
          block
        >
          Đăng nhập
        </Button>
        <hr />
        <GoogleLogin
          onSuccess={async (credentialRespose) => {
            const TokenGoogle = credentialRespose.credential;
            const resultAction = await dispatch(
              loginByGoogle({ TokenGoogle: TokenGoogle })
            );
            if (loginByGoogle.fulfilled.match(resultAction)) {
              navigate("/");
            }
          }}
        ></GoogleLogin>
      </Form.Item>
    </Form>
  );
};

export default Login;
