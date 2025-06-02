import { Form, Input, Modal, Select } from "antd";
import { useDispatch, useSelector } from "react-redux";
import { closeModal } from "../Features/Chat/chatSlice";
import FormItem from "antd/es/form/FormItem";
import { Option } from "antd/es/mentions";
// import { createChatRoom } from "../Features/Chat/chatApi";
import { getListUser } from "../Features/User/userApi";
import { createChatRoom } from "../Features/Chat/chatApi";
import { useEffect } from "react";

const ChatRoom = () => {
  const dispatch = useDispatch();
  const [form] = Form.useForm();
  const user = useSelector((state) => state.auth.userId);
  const { loading, error } = useSelector((state) => state.chat);

  const { dataUser } = useSelector((state) => state.user);
  const isOpenModal = useSelector((state) => state.chat.isOpenModal);
  const formItemLayout = {
    labelCol: {
      xs: { span: 8 },
      sm: { span: 8 },
      md: { span: 8 },
      lg: { span: 8 },
      xl: { span: 8 },
      xxl: { span: 8 },
    },
    wrapperCol: {
      xs: { span: 16 },
      sm: { span: 16 },
      md: { span: 16 },
      lg: { span: 16 },
      xl: { span: 16 },
      xxl: { span: 16 },
    },
  };
  useEffect(() => {
    if (!loading.sending && !error && isOpenModal) {
      dispatch(closeModal());
      form.resetFields();
    }
  });
  const onCancel = () => {
    form.resetFields();
    dispatch(closeModal());
  };
  const onSave = async () => {
    debugger;
    const values = await form.validateFields();
    dispatch(
      createChatRoom({
        userId: user,
        nameRoom: values.NameRoom,
        nguoiThamGia: values.nguoiThamGia,
      })
    );
    // dispatch(closeModal());
  };

  const changeNguoiThamGia = () => {
    dispatch(getListUser());
  };
  console.log("user", user);
  console.log("user", dataUser);
  return (
    <>
      <Modal
        title="Tạo nhóm chat"
        cancelText="Hủy"
        okText="Lưu"
        onCancel={onCancel}
        onOk={onSave}
        open={isOpenModal}
      >
        <Form form={form}>
          <FormItem
            label=" Tên nhóm "
            name="NameRoom"
            rules={[{ required: true, message: "Nhập tên phòng room" }]}
            {...formItemLayout}
          >
            <Input />
          </FormItem>
          <FormItem
            label="Người tham gia"
            name="nguoiThamGia"
            rules={[{ required: true, message: "Chọn người tham gia " }]}
            {...formItemLayout}
          >
            <Select
              style={{ width: "100%" }}
              placeholde={"chọn người tham gia "}
              optionFilterProp="children"
              onChange={changeNguoiThamGia}
              // value={user === "" ? undefined : user}
              allowClear
              mode="multiple"
            >
              {dataUser.map((item) => (
                <Option key={item.id} value={item.id}>
                  {item.userName}
                </Option>
              ))}
            </Select>
          </FormItem>
        </Form>
      </Modal>
    </>
  );
};
export default ChatRoom;
